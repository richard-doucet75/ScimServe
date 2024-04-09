using ScimServe.ValueTypes;

namespace ScimServe.UseCases;

[Serializable]
public class UserCredentials
{
    public required UserName UserName { get; init; }
    public required Password Password { get; init; }
}