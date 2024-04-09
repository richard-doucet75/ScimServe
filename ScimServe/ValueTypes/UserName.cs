namespace ScimServe.ValueTypes;

public sealed class UserName
{
    private readonly string _value;

    private UserName(string value)
    {
        _value = value;
    }

    public static implicit operator UserName(string value) => Create(value);

    public static implicit operator string(UserName userName) => userName._value;

    private static UserName Create(string value)
    {
        Validate(value);
        return new UserName(value);
    }

    private static void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("UserName cannot be null or whitespace.", nameof(value));
        }

        if (value.Length < 5 || value.Length > 255)
        {
            throw new ArgumentException("UserName must be between 5 and 255 characters long.", nameof(value));
        }
    }

    public override string ToString() => _value;

    public override bool Equals(object? obj)
    {
        return obj is UserName other && _value == other._value;
    }

    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(UserName? left, UserName? right) => Equals(left, right);

    public static bool operator !=(UserName? left, UserName? right) => !Equals(left, right);
}