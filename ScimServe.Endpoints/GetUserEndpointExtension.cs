using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ScimServe.Endpoints.Presenters;
using ScimServe.UseCases;

public static class GetUserEndpointExtension
{
    public static void MapGetUserEndpoint(this WebApplication app)
    {
        app.MapGet("/users/{id}", async (string id, GetUser getUserUseCase, GetUserPresenter presenter) =>
            {
                await getUserUseCase.Execute(presenter, id);
            })
            .Produces<PresentableUser>(StatusCodes.Status200OK, "application/json+scim")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status403Forbidden)
            .WithName("GetUser")
            .WithTags("Users");
    }
}