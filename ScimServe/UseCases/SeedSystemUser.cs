using LabelLocker;
using Microsoft.Extensions.Logging;
using ScimServe.Repositories;
using ScimServe.Repositories.Entities;
using ScimServe.Services;
using ScimServe.ValueTypes;

namespace ScimServe.UseCases;

public class SeedSystemUser
{
    private readonly ILogger<SeedSystemUser> _logger;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ILabelService _labelService;
    private readonly IUserRepository _userRepository;

    public SeedSystemUser(ILogger<SeedSystemUser> logger,
        IPasswordHashingService passwordHashingService,
        ILabelService labelService,
        IUserRepository userRepository)
    {
        _logger = logger;
        _passwordHashingService = passwordHashingService;
        _labelService = labelService;
        _userRepository = userRepository;
    }

    public async Task Execute(CreateUser.IPresenter presenter, SeedCredentials seedableUser)
    {
        _logger.LogInformation("Executing SeedSystemUser use case");
        byte[]? userNameReservationToken = null;

        //Ensure that there are no users created
        if (!await _userRepository.DatabaseIsSeedableForUsers())
        {
            _logger.LogWarning("Database already seeded. Cannot seed again");
            await presenter.PresentError("Database already seeded. Cannot seed again.");
            return;
        }

        // Reserve the username label
        var systemUserId = Guid.NewGuid().ToString();
        var reservationResult = await _labelService.ReserveLabelAsync(systemUserId);
        if (!reservationResult.Success)
        {
            _logger.LogWarning("Unable to reserve username: {UserName}", "SystemUser");
            await presenter.PresentError("Username is not available.");
            return;
        }

        userNameReservationToken = reservationResult.ReservationToken;
        if (userNameReservationToken is null)
        {
            _logger.LogWarning("Reservation token is null for username: {UserName}", "SystemUser");
            await presenter.PresentError("Username is not available.");
            return;
        }

        // Create the user entity
        var randomUsername = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        randomUsername = randomUsername.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            
        var userEntity = new UserEntity
        {
            Id = systemUserId,
            Key = Guid.NewGuid().ToString(), // Unique key for each new record
            UserName = randomUsername,
            UserNameLabelReservationToken = userNameReservationToken,
            ExternalId = null,
            PasswordHash = _passwordHashingService.HashPassword(seedableUser.Password),
            Version = 1, // Starting version number for a new user
            RecordTime = DateTimeOffset.UtcNow,
            RecordUserId = systemUserId,
            IsSystemUser = true
        };

        await _userRepository.InsertAsync(userEntity);

        // Present the success
        await presenter.PresentSuccess(new PresentableUser
            {
                Id = userEntity.Id,
                UserName = userEntity.UserName,
                ExternalId = userEntity.ExternalId != null 
                    ? (ExternalId)userEntity.ExternalId
                    : ExternalId.Null
            },
            userEntity.Id,
            userEntity.Version,
            userEntity.RecordTime);
    }
}