namespace ScimServe.UseCases;

public class Providable<T>
{
    public bool HasBeenProvided { get; set; }
    public T? Value { get; private set; }
    
    public Providable(T? value, bool provided)
    {
        Value = value;
        HasBeenProvided = provided;
    }
}