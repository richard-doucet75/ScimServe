using AsyncAuthFlowCore.Abstractions;
using Microsoft.Extensions.Logging;
using ScimServe.Repositories;
using ScimServe.Repositories.Entities;
using ScimServe.ValueTypes;

namespace ScimServe.UseCases;

public class GetUser
{
    public interface IPresenter
    {
        Task PresentSuccess(PresentableUser user, string id, int version, DateTimeOffset created, DateTimeOffset lastModified);
        Task PresentUserNotFound(string identifier);
        Task PresentError(string message);
        Task PresentException(Exception exception);
        Task PresentPermissionDenied();
    }

    private readonly ILogger<GetUser> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserContextConfigurator _userContext;

    public GetUser(
        ILogger<GetUser> logger,
        IUserRepository userRepository,
        IUserContextConfigurator userContext)
    {
        _logger = logger;
        _userRepository = userRepository;
        _userContext = userContext
            .RequirePermission(nameof(GetUser));
    }
    
    public async Task Execute(IPresenter presenter, string? id = null)
    {
        if (id != null)
        {
            _logger.LogInformation("Executing GetUser use case for identifier: {Identifier}", id);
        }
        else
        {
            _logger.LogInformation("Executing GetUser use case for current user");
        }
        
        await _userContext.OnPermissionGranted(async (me, _) =>
        {
            var identifier = id ?? me; 
            var user = await _userRepository.FindLatestByIdentifierAsync(identifier);
            if (user == null)
            {
                _logger.LogWarning("User not found for identifier: {Identifier}", identifier);
                await presenter.PresentUserNotFound(identifier);
                return;
            }
            
            var created = await _userRepository.GetDateCreated(identifier);
            await presenter.PresentSuccess(MapToPresentableUser(user), user.Id, user.Version, created, user.RecordTime);
        })
        .OnPermissionDenied(async (_, _) =>
        {
            await presenter.PresentPermissionDenied();
        })
        .OnException(async (exception, me, _) =>
        {
            var identifier = id ?? me; 
            _logger.LogError(exception, "Exception occurred in GetUser operation for identifier: {Identifier}", identifier);
            await presenter.PresentException(exception);
        })
        .ExecuteAsync();
    }

    private PresentableUser MapToPresentableUser(UserEntity user)
    {
        return new PresentableUser
        {
            Id = user.Id,
            UserName = user.UserName,
            ExternalId = user.ExternalId == null 
                ? ExternalId.Null 
                : (ExternalId)user.ExternalId
        };
    }
}
