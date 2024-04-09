using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using ScimServe.UseCases;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ScimServe.WebAPI.SchemaFilters;

[UsedImplicitly]
public class SeedUserSchemaFilter : ISchemaFilter
{
    private const string Password = "password";

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(SeedCredentials)) return;

        schema.Properties[Password] = new OpenApiSchema { Type = "string" };
    }
}