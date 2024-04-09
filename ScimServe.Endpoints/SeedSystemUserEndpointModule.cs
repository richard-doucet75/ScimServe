using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScimServe.Endpoints.Presenters;
using ScimServe.UseCases;
using Swashbuckle.AspNetCore.Annotations;

namespace ScimServe.Endpoints;

/// <summary>
/// Defines the endpoint module for seeding a system user within the ScimServe application.
/// This module encapsulates the setup and configuration of endpoints related to
/// the initial seeding of a system user, ensuring that the application
/// can start with necessary default user data.
/// </summary>
public static class SeedSystemUserEndpointModule
{
    /// <summary>
    /// Maps the endpoint for seeding a system user in the application.
    /// This endpoint should be called only to initialize the system with a default user
    /// and should be protected to prevent unauthorized use. It is responsible for
    /// creating the initial system user if the user database is empty, ensuring
    /// the system is ready for operation.
    /// </summary>
    /// <remarks>
    /// The endpoint is designed to accept a <see cref="SeedCredentials"/> object containing
    /// the necessary user information. This process should ensure the application's
    /// user database has its initial required data set securely and efficiently.
    /// Proper security measures should be implemented to restrict access to this endpoint
    /// to prevent unauthorized seeding of user data.
    /// </remarks>
    /// <param name="app">The <see cref="WebApplication"/> to which the endpoint will be mapped,
    /// allowing the seeding operation to be integrated into the application's request processing pipeline.</param>
    public static void MapSeedSystemUserEndpoints(this WebApplication app)
    {
        const string operationPurpose = "initial setup or environment preparation";
        
        app.MapPost("/users/system-seed", 
                async (
                    SeedSystemUser seedSystemUser, 
                    CreateUserPresenter presenter,
                    [FromBody]SeedCredentials seedSystemUserInfo) =>
            {
                // Execute the seed system user use case with the provided information.
                await seedSystemUser.Execute(presenter, seedSystemUserInfo);
            })
            .Accepts<SeedCredentials>("application/json") // Ensure the type matches the expected input
            .WithTags("Configuration")
            .Produces<PresentableUser>(StatusCodes.Status201Created, "application/json+scim")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithMetadata(new SwaggerOperationAttribute(
                summary: "Initialize Database with System User",
                description: $"Creates and seeds the application's database with an initial system user account. " +
                             $"Intended for one-time use during {operationPurpose}. " +
                             "This operation facilitates early configuration and access control by establishing " +
                             "an essential system user account."
            ))
            .WithName("SeedSystemUser");
    }
}
