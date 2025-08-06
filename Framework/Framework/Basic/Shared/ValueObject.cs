namespace Framework;

public abstract class ValueObject<T> : IEquatable<T>
    where T : ValueObject<T>
{
    public bool Equals(T? other)
       => this == other;

    public override bool Equals(object? obj)
        => obj is not null &&
        obj is T other &&
        obj.GetType() == GetType() &&
        GetEqualityProperties().SequenceEqual(other.GetEqualityProperties());

    public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        =>
        left is null && right is null ? true
            :
        left is not null && right is not null ? left.Equals(right)
        :
        false;

    public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        => !(left == right);

    public override int GetHashCode()
        => GetEqualityProperties()
                .Select(_ => _?.GetHashCode() ?? 0)
                .Aggregate((a, b) => a ^ b);

    protected abstract IEnumerable<object> GetEqualityProperties();
}

public abstract class Identity
    : ValueObject<Identity>
{ }

public sealed class Code : ValueObject<Code>
{
    public readonly Guid Value;

    Code(Guid value) => Value = value;
    Code(string value)
    {
        if (value.IsGuid(out Guid result)) Value = result;

        // exception for else
    }

    public static Code New(Guid value) => new(value);
    public static Code New(string value) => new(value);

    public static explicit operator string(Code source) => source.ToString();
    public static explicit operator Guid(Code source) => source.Value;

    public static implicit operator Code(string source) => new(source);
    public static implicit operator Code(Guid source) => new(source);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Value;
    }
}