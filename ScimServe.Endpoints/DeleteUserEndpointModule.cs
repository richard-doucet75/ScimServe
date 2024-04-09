using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScimServe.Services;
using ScimServe.UseCases;

namespace ScimServe.Endpoints;

public static class DeleteUserEndpointModule
{
    public static void MapDeleteUserEndpoints(this WebApplication app)
    {
        app.MapDelete("/users/{identifier}", 
                [Authorize] async (
                    [FromRoute]string identifier,
                    [FromHeader(Name = "ETag")]string eTag,
                    [FromServices]IETagService eTagService,
                    [FromServices]DeleteUser deleteUser,
                    [FromServices]DeleteUserPresenter presenter) =>
            {
                var (_, _, version) = eTagService.ParseETag(eTag);
                await deleteUser.Execute(presenter, identifier, version);
            })
            .WithTags("Users")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("DeleteUser");
    }
}