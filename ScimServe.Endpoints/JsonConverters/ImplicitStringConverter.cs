using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScimServe.Endpoints.JsonConverters;

public class ImplicitStringConverter<T> : JsonConverter<T>
{
    private readonly Func<string, T> _fromStringFunc;
    private readonly Func<T, string> _toStringFunc;

    public ImplicitStringConverter()
    {
        // Find the implicit operator from string to T
        var toTypeOperator = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "op_Implicit" && m.ReturnType == typeof(T) && 
                                 m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(string));
        if (toTypeOperator == null)
        {
            throw new InvalidOperationException($"No implicit conversion operator found from string to {typeof(T).Name}");
        }
        _fromStringFunc = (Func<string, T>)Delegate.CreateDelegate(typeof(Func<string, T>), toTypeOperator);

        // Find the implicit operator from T to string
        var fromTypeOperator = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "op_Implicit" && m.ReturnType == typeof(string) &&
                                 m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(T));
        if (fromTypeOperator == null)
        {
            throw new InvalidOperationException($"No implicit conversion operator found from {typeof(T).Name} to string");
        }
        _toStringFunc = (Func<T, string>)Delegate.CreateDelegate(typeof(Func<T, string>), fromTypeOperator);
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        return _fromStringFunc(stringValue);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var stringValue = _toStringFunc(value);
        writer.WriteStringValue(stringValue);
    }
}