namespace Framework;

using Microsoft.Extensions.Options;
using static System.Text.Encoding;

public interface IEncryptService
{
    EncryptResult Hash(string password);
    bool Verify(string password, string hashedPassword, string salt);




    // TODO: بهتر است که پسورد هش شده با سالت ترکیب شود و در دیتابیس به عنوان یک پراپرتی ذخیره شود
    // "HashLengthInBytes.Iterations.Salt.Password"
    // زمان وریفای کردن هم، رشته را جدا میکنیم و وریفای را انجام میدهیم
    // با اینکار، در صورت تعییر پارامترهای ایجاد پسورد مثل ایتریشن و ...  هم می توان پسورد قبلی را هم وریفای کرد
    // چون دیگر این پارامترها به صورت ثابت در کلاس ها نیستند
    //string Hash(string password);
}

public abstract class EncryptService : IEncryptService
{
    //protected readonly EncryptOption Options;

    //protected EncryptService(IOptionsMonitor<EncryptOption> options)
    //{
    //    Options = options.CurrentValue;
    //}

    protected byte[] ToBytes(string source) => UTF8.GetBytes(source);
    protected string ToString(byte[] source) => UTF8.GetString(source);
    public abstract EncryptResult Hash(string password);
    //public abstract string Hash1(string password);
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

public record EncryptOption(int HashSize, int HashLengthInBytes);

public enum EncryptFlag : byte { Argon2, Rfc, Bcrypt, Script }