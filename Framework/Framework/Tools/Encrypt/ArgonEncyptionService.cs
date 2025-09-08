namespace Framework;

using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Konscious.Security.Cryptography;

public sealed class ArgonEncryptService : EncryptService
{
    const int HashLengthInBytes = 16;
    const int HashSize = 64;
    const int Iterations = 4;
    const int MemorySizeKb = 1024 * 1024; // 1GB
    const int Parallelism = 8;

    public override EncryptResult Hash(string password)
    {
        var saltInByte = RandomNumberGenerator.GetBytes(HashLengthInBytes);
        var hashed = Argon2(password, saltInByte);
        var result = EncryptResult.New(Convert.ToHexString(hashed), Convert.ToHexString(saltInByte));
        return result;
    }

    public override bool Verify(string inputPassword, string hashedPassword, string salt)
    {
        var hashed = Argon2(inputPassword, Convert.FromHexString(salt));
        return CryptographicOperations
            .FixedTimeEquals(hashed, Convert.FromHexString(hashedPassword));
    }

    byte[] Argon2(string password, byte[] salt)
    {
        var argon2 = new Argon2id(ToBytes(password));
        argon2.Salt = salt;
        argon2.Iterations = Iterations;
        argon2.MemorySize = MemorySizeKb;
        argon2.DegreeOfParallelism = Parallelism;
        return argon2.GetBytes(HashSize);
    }
}

public interface IEncryptionService
{
    string Hash(string password);
    bool Verify(string password, string hashedPassword);
}

//public record ArgonEncryptionOption(int Iterations = 4, int HashSize = 64, int SaltSize = 16, int DegreeOfParallelism = 4);
public record ArgonEncryptionOption
{
    public ArgonEncryptionOption() { }

    public int Iterations { get; init; } = 4;
    public int DegreeOfParallelism { get; init; } = 4;
    public int SaltSize { get; init; } = 16;
    public int HashSize { get; init; } = 64;
}

public sealed class ArgonEncryptionService : IEncryptionService
{
    // -- [ HASH FORMAT: Iterations.HashSize.Parallelism.MemorySize.SaltSize.Salt.Password ] -- \\

    // برای اپ های معمولی 64 * 1024
    // برای اپ های حساس تر 256 * 1024
    const int MemorySizeKb = 131_072; // 128 MB, 128 * 1024

    private readonly ArgonEncryptionOption _options;

    //public ArgonEncryptionService(ArgonEncryptionOption options)
    //   => _options = options;

    public ArgonEncryptionService(IOptionsMonitor<ArgonEncryptionOption> options)
        => _options = options.CurrentValue;

    public string Hash(string password)
    {
        var saltInByte = RandomNumberGenerator.GetBytes(_options.SaltSize);
        var hashInByte = Argon2(password, saltInByte, _options.Iterations, _options.DegreeOfParallelism, MemorySizeKb, _options.HashSize);
        var result = $"{_options.Iterations}.{_options.HashSize}.{_options.DegreeOfParallelism}.{MemorySizeKb}.{_options.SaltSize}.{Convert.ToHexString(saltInByte)}.{Convert.ToHexString(hashInByte)}";
        return result;
    }

    public bool Verify(string password, string hashedValue)
    {
        var hashParts = hashedValue.Split('.');
        var iteration = Int32.Parse(hashParts[0]);
        var hashSize = Int32.Parse(hashParts[1]);
        var parallelism = Int32.Parse(hashParts[2]);
        var memorySize = Int32.Parse(hashParts[3]);
        var saltSize = Int32.Parse(hashParts[4]);
        var salt = Convert.FromHexString(hashParts[5]);
        var storedHashed = Convert.FromHexString(hashParts[6]);

        var newHash = Argon2(password, salt, iteration, parallelism, memorySize, hashSize);
        var result = CryptographicOperations.FixedTimeEquals(newHash, storedHashed);
        return result;
    }

    private byte[] Argon2(string password, byte[] salt, int iteration, int degreeOfParallelism, int memorySize, int hashSize)
    {
        return new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = iteration,
            MemorySize = MemorySizeKb,
            DegreeOfParallelism = degreeOfParallelism
        }.GetBytes(hashSize);
    }
}
