namespace Framework;

using System.Security.Cryptography;

public sealed class RfcEncryptionService : EncryptService
{
    const int HashLengthInBytes = 64;
    const int Iterations = 350_000;

    public override EncryptResult Hash(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(HashLengthInBytes);
        var hashed = Pbkdf2(password, saltBytes);

        return EncryptResult.New(Convert.ToHexString(hashed), Convert.ToHexString(saltBytes));
    }

    public override bool Verify(string inputPassword, string hashedPassword, string salt)
    {
        var hashed = Pbkdf2(inputPassword, Convert.FromHexString(salt));
        return CryptographicOperations
            .FixedTimeEquals(hashed, Convert.FromHexString(hashedPassword));
    }

    byte[] Pbkdf2(string password, byte[] salt)
        => Rfc2898DeriveBytes.Pbkdf2(ToBytes(password), salt, Iterations, HashAlgorithmName.SHA512, HashLengthInBytes);
}