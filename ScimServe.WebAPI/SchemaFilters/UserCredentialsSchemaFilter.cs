using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using ScimServe.UseCases;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ScimServe.WebAPI.SchemaFilters;

[UsedImplicitly]
public class UserCredentialsSchemaFilter : ISchemaFilter
{
    private const string UserName = "userName";
    private const string Password = "password";

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(UserCredentials)) return;

        schema.Properties[UserName] = new OpenApiSchema { Type = "string" };
        schema.Properties[Password] = new OpenApiSchema { Type = "string" };
    }
}