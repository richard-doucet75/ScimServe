using System.Reflection;
using AsyncAuthFlowCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScimServe.Repositories;

namespace ScimServe.EFCoreRepositories;

public static class ScimServeEfCoreInstaller
{
    public static void ConfigureEfCore(this IServiceCollection services, IConfiguration configuration, Assembly migrationAssembly)
    {
        var connectionString = configuration.GetConnectionString("ScimDatabase") 
                               ?? throw new InvalidOperationException("The SQL Server connection string 'ScimDatabase' is not configured.");

        services.AddDbContext<ScimDbContext>(options =>
            options.UseSqlServer(connectionString,
            b => b.MigrationsAssembly(migrationAssembly.FullName)));
            
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserPermissionsRepository, UserRepository>();
    }
}