namespace ScimServe.ValueTypes;

public readonly struct ExternalId
{
    private string Value { get; }

    private ExternalId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("External ID cannot be null or whitespace.", nameof(value));
        }

        Value = value;
    }

    // Implicit conversion from string to ExternalId
    public static implicit operator ExternalId(string value) 
        => new(value);

    // Implicit conversion from ExternalId to string
    public static implicit operator string(ExternalId externalId) 
        => externalId.Value;

    // Override ToString to return the string representation
    public override string ToString() => Value;
    
    // Optionally implement equality based on Value
    public override bool Equals(object? obj)
    {
        return obj is ExternalId other && this.Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    // Equality operators if needed
    public static bool operator ==(ExternalId left, ExternalId right) => Equals(left, right);
    public static bool operator !=(ExternalId left, ExternalId right) => !Equals(left, right);
    public static ExternalId? Null => null;
}
