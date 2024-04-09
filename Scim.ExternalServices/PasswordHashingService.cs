using ScimServe.Services;


namespace Scim.ExternalServices;

public class PasswordHashingService : IPasswordHashingService
{
    public string HashPassword(string password)
    {
        // Enhanced security with BCrypt using work factor (log rounds)
        // The higher the work factor, the longer it takes to hash and verify a password
        // Adjust the work factor based on your application's performance and security needs
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // Using BCrypt to check the password against the hashed password
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    } 
}