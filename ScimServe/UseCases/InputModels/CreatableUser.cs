using ScimServe.ValueTypes;

namespace ScimServe.UseCases;

[Serializable]
public class CreatableUser
{
    public ExternalId? ExternalId { get; init; }
    public required UserName UserName { get; init; }
    public required Password Password { get; init; }
}