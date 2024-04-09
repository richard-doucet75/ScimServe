using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ScimServe.WebAPI.DocumentFilter;

[UsedImplicitly]
public class ExcludeTypesDocumentFilter : IDocumentFilter
{

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        SwaggerConfig.TypesToExclude.ForEach(type =>
        {
            context.SchemaRepository.Schemas.Remove(type.Name);
        });
    }
}