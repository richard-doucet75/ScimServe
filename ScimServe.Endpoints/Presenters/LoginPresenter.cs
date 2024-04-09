using Microsoft.AspNetCore.Http;
using ScimServe.Endpoints.Services;
using ScimServe.UseCases;

namespace ScimServe.Endpoints.Presenters;

public class LoginPresenter : Login.IPresenter
{
    private const string ContentType = "application/json";
    
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IResponseService _responseService;

    public LoginPresenter(IJwtTokenGenerator jwtTokenGenerator, IResponseService responseService)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _responseService = responseService;
    }

    public async Task PresentAccessGranted(PresentableLogin login)
    {
        _responseService.SetResponse(StatusCodes.Status201Created, ContentType, _jwtTokenGenerator.GenerateTokenForUserId(login.UserId));
    }

    public Task PresentAccessDenied()
    {
        _responseService.SetResponse(StatusCodes.Status401Unauthorized, ContentType, "Access Denied");
        return Task.CompletedTask;
    }

    public Task PresentException(Exception exception)
    {
        _responseService.SetResponse(StatusCodes.Status500InternalServerError, ContentType, "An error occurred");
        return Task.CompletedTask;
    }
}