using ScimServe.ValueTypes;

namespace ScimServe.UseCases;

[Serializable]
public class PresentableUser
{
    public required string Id { get; set; }
    public required UserName UserName { get; init; }
    public ExternalId? ExternalId { get; init; }
}