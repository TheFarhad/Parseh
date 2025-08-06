namespace Framework;

using System.Security.Cryptography;
using Konscious.Security.Cryptography;

public sealed class ArgonEncyptionService : EncryptService
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
