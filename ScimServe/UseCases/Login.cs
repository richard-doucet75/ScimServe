using Microsoft.Extensions.Logging;
using ScimServe.Repositories;
using ScimServe.Services;

namespace ScimServe.UseCases;

public class Login
{
    private readonly ILogger<Login> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;

    public interface IPresenter
    {
        Task PresentAccessGranted(PresentableLogin login);
        Task PresentAccessDenied();
        Task PresentException(Exception exception);
    }

    
    public Login(
        ILogger<Login> logger,
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
    }

    public async Task Execute(IPresenter presenter, UserCredentials authenticatable)
    {
        try
        {
            var user = await _userRepository.FindByUsernameAsync(authenticatable.UserName);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("Login attempt for non-existent or deleted user: {Username}", authenticatable.UserName);
                await presenter.PresentAccessDenied();
                return;
            }

            var passwordMatches = _passwordHashingService.VerifyPassword(authenticatable.Password, user.PasswordHash);
            if (!passwordMatches)
            {
                _logger.LogWarning("Invalid login attempt for user: {Username}", authenticatable.UserName);
                await presenter.PresentAccessDenied();
                return;
            }

            // Assuming successful login at this point
            await presenter.PresentAccessGranted(new PresentableLogin { UserId = user.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the login process for user: {Username}", authenticatable.UserName);
            await presenter.PresentException(ex);
        }
    }
}