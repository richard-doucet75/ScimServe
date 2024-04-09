using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using ScimServe.Endpoints.JsonConverters;
using ScimServe.Endpoints.Presenters;
using ScimServe.Endpoints.Services;
using ScimServe.Services;
using ScimServe.ValueTypes;

namespace ScimServe.Endpoints;

public static class EndpointsInstaller
{
    public static void ConfigureEndpoints(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        
        services.Configure<JsonOptions>(options =>
        {
            var serializerOptions = JsonSerializerOptionsFactory.Create();
            foreach (var converter in serializerOptions.Converters)
            {
                options.SerializerOptions.Converters.Add(converter);
            }
        });


        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IResponseService, HttpResponseService>();
        services.AddScoped<IUriGeneratorService, UriGeneratorService>();
        services.AddSingleton<IETagService, ETagService>();

        services.AddScoped<LoginPresenter>();
        services.AddScoped<CreateUserPresenter>();
        services.AddScoped<GetUserPresenter>();
        services.AddScoped<DeleteUserPresenter>();
    }
}
