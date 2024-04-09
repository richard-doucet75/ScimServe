namespace ScimServe.ValueTypes;

using System;
using System.Linq;

public class Password
{
    private readonly string _plaintextValue;

    private Password(string value)
    {
        ValidatePassword(value);
        _plaintextValue = value;
    }

    // Implicit operator to allow initialization from string
    public static implicit operator Password(string value) => new(value);
    public static implicit operator string(Password value) => value._plaintextValue;

    private static void ValidatePassword(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long.");
        }

        if (!value.Any(char.IsUpper))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter.");
        }

        if (!value.Any(char.IsLower))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter.");
        }

        if (!value.Any(char.IsDigit))
        {
            throw new ArgumentException("Password must contain at least one digit.");
        }

        if (!value.Any(ch => "!@#$%^&*()_+-=[]{}|;:'\",.<>/?".Contains(ch)))
        {
            throw new ArgumentException("Password must contain at least one special character.");
        }
    }
    
    // Overriding ToString to return a protected message instead of actual password value
    public override string ToString() => "Password [Protected]";
}