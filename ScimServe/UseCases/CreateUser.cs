using AsyncAuthFlowCore.Abstractions;
using LabelLocker;
using Microsoft.Extensions.Logging;
using ScimServe.Repositories;
using ScimServe.Repositories.Entities;
using ScimServe.Services;
using ScimServe.ValueTypes;

namespace ScimServe.UseCases;

public class CreateUser
{
    public interface IPresenter
    {
        Task PresentSuccess(PresentableUser user, string id, int version, DateTimeOffset createdDate);
        Task PresentError(string message);
        Task PresentException(Exception exception);
        Task PresentPermissionDenied();
    }

    private readonly ILogger<CreateUser> _logger;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ILabelService _labelService;
    private readonly IUserRepository _userRepository;
    private readonly IUserContextConfigurator _userContext;

    public CreateUser(ILogger<CreateUser> logger,
        IPasswordHashingService passwordHashingService,
        ILabelService labelService,
        IUserRepository userRepository,
        IUserContextConfigurator userContext)
    {
        _logger = logger;
        _passwordHashingService = passwordHashingService;
        _labelService = labelService;
        _userRepository = userRepository;
        _userContext = userContext
            .RequirePermission(nameof(CreateUser));
    }

    public async Task Execute(IPresenter presenter, CreatableUser newUser)
    {
        _logger.LogInformation("Executing CreateUser use case");
        byte[]? userNameReservationToken = null;
        
        await _userContext.OnPermissionGranted(async (currentUserId, _) =>
        {
            // Reserve the username label
            var reservationResult = await _labelService.ReserveLabelAsync(newUser.UserName);
            if (!reservationResult.Success)
            {
                _logger.LogWarning("Unable to reserve username: {UserName}", newUser.UserName);
                await presenter.PresentError("Username is not available.");
                return;
            }
            
            userNameReservationToken = reservationResult.ReservationToken;
            if (userNameReservationToken is null)
            {
                _logger.LogWarning("Reservation token is null for username: {UserName}", newUser.UserName);
                await presenter.PresentError("Username is not available.");
                return;
            }
            
            // Create the user entity
            var userEntity = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString(), // Unique key for each new record
                UserName = newUser.UserName,
                UserNameLabelReservationToken = userNameReservationToken,
                ExternalId = newUser.ExternalId,
                PasswordHash = _passwordHashingService.HashPassword(newUser.Password),                
                Version = 1, // Starting version number for a new user
                RecordTime = DateTimeOffset.UtcNow,
                RecordUserId = currentUserId // Set the RecordUserId to the current user's ID
            };

            await _userRepository.InsertAsync(userEntity);

            // Present the success
            await presenter.PresentSuccess(new PresentableUser
                {
                    Id = userEntity.Id,
                    UserName = userEntity.UserName,
                    ExternalId = userEntity.ExternalId != null 
                        ? (ExternalId)userEntity.ExternalId
                        : ExternalId.Null,
                },
                userEntity.Id,
                userEntity.Version,
                userEntity.RecordTime);
        })
        .OnPermissionDenied(async (userId, _) =>
        {
            _logger.LogWarning("Permission denied for user: {UserId}", userId);
            await presenter.PresentPermissionDenied();
        })
        .OnException(async (exception, _, _) =>
        {
            _logger.LogError(exception, "Exception occurred during user creation");
            if(userNameReservationToken != null)
            {
                await _labelService.ReleaseLabelAsync(newUser.UserName, userNameReservationToken);
            }
            await presenter.PresentException(exception);
        })
        .ExecuteAsync();
    }
}
