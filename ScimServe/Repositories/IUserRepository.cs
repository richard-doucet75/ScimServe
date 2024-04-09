using ScimServe.Repositories.Entities;

namespace ScimServe.Repositories
{
    /// <summary>
    /// Defines the contract for a user repository.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Finds the most recent user that matches the provided identifier.
        /// </summary>
        /// <param name="identifier">The identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the UserEntity if found; otherwise, null.</returns>
        Task<UserEntity?> FindLatestByIdentifierAsync(string identifier);

        /// <summary>
        /// Finds a user that matches the provided username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the UserEntity if found; otherwise, null.</returns>
        Task<UserEntity?> FindByUsernameAsync(string username);

        /// <summary>
        /// Inserts a new user into the repository.
        /// </summary>
        /// <param name="newUser">The new user to insert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating the success of the operation.</returns>
        Task<bool> InsertAsync(UserEntity newUser);

        /// <summary>
        /// Checks if the database can be seeded with users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the database can be seeded with users.</returns>
        Task<bool> DatabaseIsSeedableForUsers();

        Task<DateTimeOffset> GetDateCreated(string identifier);
    }
}