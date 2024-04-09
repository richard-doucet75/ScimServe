using ScimServe.Endpoints.Presenters.Models;

namespace ScimServe.Endpoints.Services;

/// <summary>
/// Defines the contract for a service that generates JSON Web Tokens (JWT) for user authentication.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token encapsulated in a TokenInfo object for a specified user.
    /// </summary>
    /// <param name="loginUserId">The identifier of the user for whom the token is being generated.</param>
    /// <returns>A TokenInfo object containing the JWT token, token type, refresh token (if implemented), and expiration time.</returns>
    TokenInfo GenerateTokenForUserId(string loginUserId);
}