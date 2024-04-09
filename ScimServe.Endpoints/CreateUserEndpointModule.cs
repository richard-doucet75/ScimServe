using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ScimServe.UseCases;

namespace ScimServe.Endpoints;

public static class CreateUserEndpointModule
{
    public static void MapCreateUserEndpoints(this WebApplication app)
    {
        app.MapPost("/users", [Authorize] async (CreateUser useCase, 
                CreatableUser creatableUser, CreateUserPresenter presenter) =>
            {
                await useCase.Execute(presenter, creatableUser);
            })
            .Accepts<CreatableUser>("application/json+scim")
            .WithTags("Users")
            .Produces<PresentableUser>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("CreateUser");
    }
}