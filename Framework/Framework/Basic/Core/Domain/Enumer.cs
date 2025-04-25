namespace Framework;

using System.Diagnostics;

[DebuggerDisplay("{Display}")]
public abstract class Enumer : Atom, IComparable
{
    public readonly string Value;
    public abstract string Display { get; }

    protected Enumer(string value) => Value = value.IsNotEmpty() ? value/*.ToLower()*/ : String.Empty;
    protected Enumer() : this(Empty) { }

    public List<T>? Items<T>() where T : Enumer
       => typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Single(_ => _.Name == "Items")
                .GetValue(null)?
                .As<List<T>>();

    public override bool Equals(object? obj)
    {
        var result = false;
        if (this is null && obj is null)
            result = true;
        else if (this is { } a && obj?.As<Enumer>() is { } b)
            result = String.Equals(a.Value, b.Value, StringComparison.OrdinalIgnoreCase);
        return result;
    }

    public static bool operator ==(Enumer a, Enumer b) => a.Equals(b);
    public static bool operator !=(Enumer a, Enumer b) => !(a == b);

    public int CompareTo(object? obj) => Display.CompareTo(obj?.As<Enumer>().Display);

    public override int GetHashCode() => base.GetHashCode();

}