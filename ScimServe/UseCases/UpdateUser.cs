using AsyncAuthFlowCore.Abstractions;
using LabelLocker;
using Microsoft.Extensions.Logging;
using ScimServe.Repositories;
using ScimServe.Repositories.Entities;
using ScimServe.Services;

namespace ScimServe.UseCases;

public class UpdateUser
{
    public interface IPresenter
    {
        Task PresentSuccess(PresentableUser user, int version);
        Task PresentPermissionDenied();
        Task PresentUserNotFound(string identifier);
        Task PresentError(string message);
        Task PresentException(Exception exception);
    }

    private readonly ILogger<UpdateUser> _logger;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ILabelService _labelService;
    private readonly IUserRepository _userRepository;
    private readonly IUserContextConfigurator _userContext;

    public UpdateUser(
        ILogger<UpdateUser> logger,
        IPasswordHashingService passwordHashingService,
        ILabelService labelService,
        IUserRepository userRepository,
        IUserContextConfigurator userContext)
    {
        _logger = logger;
        _passwordHashingService = passwordHashingService;
        _labelService = labelService;
        _userRepository = userRepository;
        _userContext = userContext.RequirePermission(nameof(UpdateUser));
    }
    
    public async Task Execute(IPresenter presenter, string identifier, UpdatableUser updatedUser, int expectedVersion)
    {
        _logger.LogInformation("Executing UpdateUser use case for identifier: {Identifier}", identifier);

        await _userContext.OnPermissionGranted(async (currentUserId, _) =>
        {
            var existingUser = await _userRepository.FindLatestByIdentifierAsync(identifier);
            if (existingUser == null)
            {
                _logger.LogWarning("User not found for identifier: {Identifier}", identifier);
                await presenter.PresentUserNotFound(identifier);
                return;
            }

            if (existingUser.Version != expectedVersion)
            {
                await presenter.PresentError("The user has been modified by another transaction. Please reload and try again.");
                return;
            }

            var userName = GetValueToUse(updatedUser.UserName, existingUser.UserName)!;
            var userNameLabelReservationToken = existingUser.UserNameLabelReservationToken;
            if (!userName.Equals(existingUser.UserName))
            {
                var reservationResult = await _labelService.ReserveLabelAsync(userName);
                if (!reservationResult.Success)
                {
                    _logger.LogWarning("Unable to reserve new username: {UserName}", userName);
                    await presenter.PresentError("Username is not available.");
                    return;
                }

                userNameLabelReservationToken = reservationResult.ReservationToken
                                                ?? throw new InvalidOperationException(
                                                    "Reservation token must be provided.");

                // Release the previous username after successfully reserving the new one
                await _labelService.ReleaseLabelAsync(existingUser.UserName, existingUser.UserNameLabelReservationToken);
            }
            
            var externalId = GetValueToUse(updatedUser.ExternalId, existingUser.ExternalId);
            var passwordHash = GetValueToUse(updatedUser.Password, existingUser.PasswordHash,
                password => _passwordHashingService.HashPassword(password))!;
            
            var newUser = new UserEntity
            {
                Key = Guid.NewGuid().ToString(), 
                Id = existingUser.Id,
                Version = existingUser.Version + 1,
                UserName = userName,
                UserNameLabelReservationToken = userNameLabelReservationToken,
                ExternalId = externalId,
                PasswordHash = passwordHash,
                RecordTime = DateTimeOffset.UtcNow,
                RecordUserId = currentUserId
            };


            var insertSucceeded = await _userRepository.InsertAsync(newUser);
            if (!insertSucceeded)
            {
                _logger.LogError("Optimistic locking failure or other update failure for user: {Identifier}", identifier);
                await presenter.PresentError("Update failed due to concurrent modification or other error. Please try again.");
                return;
            }

            await presenter.PresentSuccess(MapToPresentableUser(newUser), newUser.Version);
        })
        .OnPermissionDenied(async (_, _) => await presenter.PresentPermissionDenied())
        .OnException(async (exception, _, _) =>
        {
            _logger.LogError(exception, "Exception occurred in UpdateUser operation for identifier: {Identifier}", identifier);
            await presenter.PresentException(exception);
        })
        .ExecuteAsync();
    }

    private static string? GetValueToUse<T>(Providable<T> providable, string? existing,
        Func<T, string>? valueExtractor = null)
    {
        if (!providable.HasBeenProvided) return existing;
        if (providable.Value == null)
        {
            throw new InvalidOperationException($"When provided, a {nameof(T)} must not be null.");
        }

        return valueExtractor != null
            ? valueExtractor(providable.Value)
            : providable.Value.ToString();
    }

    private PresentableUser MapToPresentableUser(UserEntity user)
    {
        return new PresentableUser
        {
            Id = user.Id,
            UserName = user.UserName,
            ExternalId = user.ExternalId!
        };
    }
}
