using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using ScimServe.UseCases;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ScimServe.WebAPI.SchemaFilters;

[UsedImplicitly]
public class PresentableUserSchemaFilter : IncludesMetaDataSchemaFilter<PresentableUser>
{
    private const string Id = "id";
    private const string UserName = "userName";
    private const string ExternalId = "externalId";
    
    public override void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(PresentableUser)) return;

        schema.Title = "UserResult";
        schema.Properties[Id] = new OpenApiSchema { Type = "string", Nullable = false };
        schema.Properties[UserName] = new OpenApiSchema { Type = "string" };
        schema.Properties[ExternalId] = new OpenApiSchema { Type = "string", Nullable = true };
        
        base.Apply(schema, context);
    }
}