namespace ScimServe.Endpoints.Presenters.Models;

/// <summary>
/// Represents authentication token information, including the type of token,
/// the token itself, a refresh token, and the token's expiration time in seconds.
/// </summary>
/// <param name="TokenType">The type of the token, typically "Bearer".</param>
/// <param name="Token">The token value, used for authentication.</param>
/// <param name="ExpiresIn">The expiration time of the token in seconds.</param>
public record TokenInfo(string TokenType, string Token, int ExpiresIn);