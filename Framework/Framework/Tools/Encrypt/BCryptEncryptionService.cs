namespace Framework;

using static BCrypt.Net.BCrypt;

public sealed class BCryptEncryptionService : EncryptService
{
    private const int WorkFactor = 13;

    public override EncryptResult Hash(string password)
    {
        var salt = GenerateSalt(WorkFactor);
        var hashed = HashPassword(password, salt, true);
        return EncryptResult.New(hashed, salt);
    }

    /// <summary>
    /// Salt must be null
    /// </summary>
    /// <returns></returns>
    public override bool Verify(string password, string hashedPassword, string salt = default!)
        => EnhancedVerify(password, hashedPassword);
}