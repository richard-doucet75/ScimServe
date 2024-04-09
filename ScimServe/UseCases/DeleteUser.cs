using LabelLocker;
using Microsoft.Extensions.Logging;
using ScimServe.Repositories;
using AsyncAuthFlowCore.Abstractions; // Ensure this namespace correctly refers to where your abstractions are defined.

namespace ScimServe.UseCases;

public class DeleteUser
{
    public interface IPresenter
    {
        Task PresentSuccess();
        Task PresentUserNotFound(string identifier);
        Task PresentVersionConflict(string identifier, int expectedVersion, int actualVersion);
        Task PresentError(string message);
        Task PresentException(Exception exception);
        Task PresentSystemUserDeletionAttempt();
    }

    private readonly ILogger<DeleteUser> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ILabelService _labelService;
    private readonly IUserContextConfigurator _userContext; // Adjusted for correct interface

    public DeleteUser(
        ILogger<DeleteUser> logger, 
        IUserRepository userRepository, 
        ILabelService labelService,
        IUserContextConfigurator userContext) // Adjusted for dependency injection
    {
        _logger = logger;
        _userRepository = userRepository;
        _labelService = labelService;
        _userContext = userContext;
    }

    public async Task Execute(IPresenter presenter, string identifier, int expectedVersion)
    {
        await _userContext.RequirePermission(nameof(DeleteUser)) // Using nameof for permission
            .OnPermissionGranted(async (_, _) =>
            {
                var existingUser = await _userRepository.FindLatestByIdentifierAsync(identifier);
                if (existingUser == null)
                {
                    _logger.LogWarning("Attempted to delete a user that does not exist. Identifier: {Identifier}", identifier);
                    await presenter.PresentUserNotFound(identifier);
                    return;
                }
                
                // Prevent deletion of system user
                if (existingUser.IsSystemUser)
                {
                    await presenter.PresentSystemUserDeletionAttempt();
                    return;
                }

                if (existingUser.Version != expectedVersion)
                {
                    _logger.LogWarning("Version conflict detected. Identifier: {Identifier}, Expected Version: {ExpectedVersion}, Actual Version: {ActualVersion}", identifier, expectedVersion, existingUser.Version);
                    await presenter.PresentVersionConflict(identifier, expectedVersion, existingUser.Version);
                    return;
                }

                if (existingUser.IsDeleted)
                {
                    _logger.LogInformation("User is already marked as deleted. Identifier: {Identifier}", identifier);
                    await presenter.PresentSuccess();
                    return;
                }

                var newUser = existingUser with {
                    Version = existingUser.Version + 1,
                    IsDeleted = true,
                    Key = Guid.NewGuid().ToString(), // Generate a new key for the "insert-only" model
                    RecordTime = DateTimeOffset.UtcNow
                };

                await _userRepository.InsertAsync(newUser);

                // Release the username after successfully inserting the new user record, assuming it should be released upon deletion
                var releaseResult = await _labelService.ReleaseLabelAsync(existingUser.UserName, existingUser.UserNameLabelReservationToken);
                if (!releaseResult.Success)
                {
                    _logger.LogError("Unable to release username: {UserName}", existingUser.UserName);
                }

                _logger.LogInformation("User marked as deleted, version incremented, and username released. Identifier: {Identifier}, New Version: {Version}", identifier, newUser.Version);
                await presenter.PresentSuccess();
            })
            .OnPermissionDenied(async (_, _) =>
            {
                _logger.LogWarning("Permission denied for deleting user. Identifier: {Identifier}", identifier);
                await presenter.PresentError("Permission denied.");
            })
            .OnException(async (exception, _, _) =>
            {
                _logger.LogError(exception, "Exception occurred while attempting to delete user. Identifier: {Identifier}", identifier);
                await presenter.PresentException(exception);
            })
            .ExecuteAsync();
    }
}
