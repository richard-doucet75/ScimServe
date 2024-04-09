using System;

namespace ScimServe.ValueTypes;

public sealed class Email
{
    private readonly string _value;

    private Email(string value)
    {
        _value = value;
    }

    // Extracts the domain part of the email
    public string Domain => _value[(_value.IndexOf('@') + 1)..];

    // Extracts the username part of the email (before the '@')
    public string UserName => _value.Substring(0, _value.IndexOf('@'));

    // Implicit operator to convert a string to an Email object
    public static implicit operator Email(string value) => Create(value);

    // Implicit operator to convert an Email object back to a string
    public static implicit operator string(Email email) => email._value;

    private static Email Create(string value)
    {
        Validate(value);
        return new Email(value);
    }

    private static void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be null or whitespace.", nameof(value));
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(value);
            if (addr.Address != value)
            {
                throw new ArgumentException("Invalid email address format.", nameof(value));
            }
        }
        catch
        {
            throw new ArgumentException("Invalid email address format.", nameof(value));
        }
    }

    public override string ToString() => _value;

    public override bool Equals(object? obj)
    {
        return obj is Email other && _value == other._value;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(Email? left, Email? right) => Equals(left, right);

    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);
}