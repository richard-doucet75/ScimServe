using ScimServe.ValueTypes;

namespace ScimServe.UseCases;

[Serializable]
public class SeedCredentials
{
    public required Password Password { get; init; }
}