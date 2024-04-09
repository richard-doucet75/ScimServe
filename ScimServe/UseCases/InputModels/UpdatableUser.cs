using ScimServe.ValueTypes;

namespace ScimServe.UseCases;

public class UpdatableUser
{
    public required Providable<UserName> UserName { get; set; }
    public required Providable<ExternalId?> ExternalId { get; set; }
    public required Providable<Password> Password { get; set; } 
}