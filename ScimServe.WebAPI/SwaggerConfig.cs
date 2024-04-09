using ScimServe.ValueTypes;

namespace ScimServe.WebAPI;

public static class SwaggerConfig
{
    public static List<Type> TypesToExclude { get; } = new();

    static SwaggerConfig()
    {
        TypesToExclude.Add(typeof(UserName));
        TypesToExclude.Add(typeof(Password));
        TypesToExclude.Add(typeof(ExternalId));
    }
}