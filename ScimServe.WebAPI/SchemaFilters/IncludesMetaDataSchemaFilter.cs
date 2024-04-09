using JetBrains.Annotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ScimServe.WebAPI.SchemaFilters;

[UsedImplicitly]
public class IncludesMetaDataSchemaFilter<TContent> : ISchemaFilter
{
    public virtual void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Check if the current schema is for a type that matches TContent
        if (context.Type == typeof(TContent))
        {
            // Assume metadata is added in a consistent structure, like a 'meta' property
            // First, ensure the schema's properties dictionary is initialized
            schema.Properties ??= new Dictionary<string, OpenApiSchema>();

            // Add or update the 'meta' property to reflect the expected TMetaData structure
            // This example demonstrates adding a simple metadata schema. You may need to
            // dynamically generate this schema based on TMetaData properties.
            schema.Properties["meta"] = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    // Example metadata properties
                    ["resourceType"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("User") },
                    ["created"] = new OpenApiSchema { Type = "string", Format = "date-time" },
                    ["lastModified"] = new OpenApiSchema { Type = "string", Format = "date-time" },
                    ["location"] = new OpenApiSchema { Type = "string", Format = "uri" },
                    ["version"] = new OpenApiSchema { Type = "string" }
                },
                Description = "Metadata associated with the content"
            };
        }
    }
}