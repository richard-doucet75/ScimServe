using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ScimServe.Endpoints.Presenters.Models;

namespace ScimServe.Endpoints.Services;

/// <summary>
/// Provides functionality to generate JWT (JSON Web Token) for user authentication, 
/// utilizing application configuration settings for customizable token properties.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private const string TokenType = "Bearer";
    private const string DefaultExpiryInHours = "24";
    private const string JwtExpiryConfigurationKey = "Jwt:ExpiryHours";
    private const string JwtKeyConfigurationKey = "Jwt:Key";
    private const string JwtIssuerConfigurationKey =  "Jwt:Issuer";
    private const string JwtAudienceConfiguration = "Jwt:Audience";
    
    private const string JwtKeyNotConfiguredErrorMessage = "Jwt:Key is not configured.";
    private const string JwtIssuerNotConfiguredErrorMessage = "Jwt:Issuer is not configured.";
    private const string JwtAudienceNotConfiguredErrorMessage = "Jwt:Audience is not configured.";
    
    private readonly IConfiguration _configuration;
    private readonly int _expiryHours;

    /// <summary>
    /// Initializes a new instance of the JwtTokenGenerator class, reading configuration settings
    /// for key, issuer, audience, and token expiry duration.
    /// </summary>
    /// <param name="configuration">The configuration settings used to retrieve JWT settings such as the secret key and token expiry.</param>
    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
        _expiryHours = int.Parse(_configuration[JwtExpiryConfigurationKey] ?? DefaultExpiryInHours); 
    }

    /// <summary>
    /// Generates a JWT for a specified user ID, incorporating claims for the subject and a unique identifier.
    /// Configurable settings determine the issuer, audience, and token expiration.
    /// </summary>
    /// <param name="userId">The user ID for which the token is being generated.</param>
    /// <returns>A TokenInfo object containing the generated JWT, token type, and expiration time.</returns>
    /// <exception cref="InvalidOperationException">Thrown if JWT configuration settings are not properly configured.</exception>
    public TokenInfo GenerateTokenForUserId(string userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[JwtKeyConfigurationKey]
            ?? throw new InvalidOperationException(JwtKeyNotConfiguredErrorMessage)));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiry = DateTime.UtcNow.AddHours(_expiryHours);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration[JwtIssuerConfigurationKey] 
                    ?? throw new  InvalidOperationException(JwtIssuerNotConfiguredErrorMessage),
            audience: _configuration[JwtAudienceConfiguration]
                      ?? throw new InvalidOperationException(JwtAudienceNotConfiguredErrorMessage),
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenInfo(
            TokenType: TokenType,
            Token: encodedToken,
            ExpiresIn: (int)(expiry - DateTime.UtcNow).TotalSeconds
        );
    }
}
