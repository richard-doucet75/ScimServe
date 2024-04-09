using Microsoft.Extensions.DependencyInjection;
using ScimServe.UseCases;

namespace ScimServe;

public static class ScimServeServiceCollection
{
    public static void AddScimServe(this IServiceCollection services)
    {
        services.AddScoped<CreateUser>();
        services.AddScoped<DeleteUser>();
        services.AddScoped<GetUser>();
        services.AddScoped<Login>();
        // services.AddScoped<UpdateUser>();
        services.AddScoped<SeedSystemUser>();
    }
}