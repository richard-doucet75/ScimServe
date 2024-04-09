using System.Text.Json;
using ScimServe.Endpoints.JsonConverters;
using ScimServe.ValueTypes;

namespace ScimServe.Endpoints.Services;

public static class JsonSerializerOptionsFactory
{
    public static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        // Add all necessary converters
        options.Converters.Add(new ImplicitStringConverter<UserName>());
        options.Converters.Add(new ImplicitStringConverter<Password>());
        options.Converters.Add(new ImplicitStringConverter<ExternalId>());

        return options;
    }
}