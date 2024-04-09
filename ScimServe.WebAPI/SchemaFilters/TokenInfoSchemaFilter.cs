using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using ScimServe.Endpoints.Presenters.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ScimServe.WebAPI.SchemaFilters;

[UsedImplicitly]
public class TokenInfoSchemaFilter : ISchemaFilter
{
    private const string TokenType = "tokenType";
    private const string Token = "token";
    private const string ExpiresIn = "expiresIn";

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(TokenInfo)) return;

        schema.Properties[TokenType] = new OpenApiSchema { Type = "string", Nullable = false };
        schema.Properties[Token] = new OpenApiSchema { Type = "string", Nullable = false };
        schema.Properties[ExpiresIn] = new OpenApiSchema { Type = "string",Nullable = false  };

    }
}

