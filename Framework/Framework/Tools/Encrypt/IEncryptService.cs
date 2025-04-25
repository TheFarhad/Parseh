namespace Framework;

using static System.Text.Encoding;

public interface IEncryptService
{
    EncryptResult Hash(string password);
    bool Verify(string password, string hashedPassword, string salt);
}

public abstract class EncryptService : IEncryptService
{
    protected byte[] ToBytes(string source) => UTF8.GetBytes(source);
    protected string ToString(byte[] source) => UTF8.GetString(source);
    public abstract EncryptResult Hash(string password);
    public abstract bool Verify(string password, string hashedPassword, string salt);
}

public readonly record struct EncryptResult
{
    public readonly string Hashed { get; }
    public readonly string Salt { get; }

    private EncryptResult(string hashed, string salt)
    {
        Hashed = hashed;
        Salt = salt;
    }

    public static EncryptResult New(string hashed, string salt) => new(hashed, salt);
}

public enum EncryptFlag : byte { Rfc, Bcrypt, Script }