using System.ComponentModel.DataAnnotations;

namespace ScimServe.Repositories.Entities
{
    /// <summary>
    /// Represents a user entity in the system.
    /// </summary>
    public record UserEntity
    {
        /// <summary>
        /// Gets the identifier of the user.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Gets the key of the user.
        /// </summary>
        [Key]
        public required string Key { get; init; }

        /// <summary>
        /// Gets the version of the user.
        /// </summary>
        public required int Version { get; init; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public required string UserName { get; init; }

        /// <summary>
        /// Gets the username label reservation token of the user.
        /// </summary>
        public required byte[] UserNameLabelReservationToken { get; init; }

        /// <summary>
        /// Gets the external identifier of the user.
        /// </summary>
        public string? ExternalId { get; init; }

        /// <summary>
        /// Gets the password hash of the user.
        /// </summary>
        public required string PasswordHash { get; init; }

        /// <summary>
        /// Gets a value indicating whether the user is deleted.
        /// </summary>
        public bool IsDeleted { get; init; }

        /// <summary>
        /// Gets the record time of the user.
        /// </summary>
        public required DateTimeOffset RecordTime { get; init; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets the record user identifier of the user.
        /// </summary>
        public required string RecordUserId { get; init; }

        /// <summary>
        /// Gets a value indicating whether the user is a system user.
        /// </summary>
        public bool IsSystemUser { get; init; }
    }
}