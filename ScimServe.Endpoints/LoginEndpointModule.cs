using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScimServe.Endpoints.Presenters;
using ScimServe.Endpoints.Presenters.Models;
using ScimServe.UseCases;
using Swashbuckle.AspNetCore.Annotations;

namespace ScimServe.Endpoints;

/// <summary>
/// Configures endpoints related to user login functionality within the ScimServe application.
/// </summary>
public static class LoginEndpointModule
{
    /// <summary>
    /// Maps login-related endpoints to the WebApplication, enabling user authentication.
    /// </summary>
    /// <param name="app">The WebApplication to configure.</param>
    public static void MapLoginEndpoints(this WebApplication app)
    {
        app.MapPost("/users/login",
                async (
                    Login loginUseCase,
                    [FromBody] UserCredentials userCredentials,
                    LoginPresenter presenter,
                    HttpRequest request) =>
                {
                    // Executes the login use case with provided credentials and presenter.
                    await loginUseCase.Execute(presenter, userCredentials);
                })
            .Accepts<UserCredentials>("application/json")
            .WithTags("Authentication")
            .WithName("UserLogin")
            .Produces<TokenInfo>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithMetadata(new SwaggerOperationAttribute(
                summary: "Authenticate User",
                description:
                "Authenticates a user with the provided credentials and returns user information upon successful authentication."));

    }
}
