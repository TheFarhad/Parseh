namespace Parseh.Server.Core.Domain.Aggregates.User.Entity;

using Framework;
using ValueObject;

public sealed class RefreshToken : Entity<RefreshTokenId>
{
    private RefreshToken() { }
    private RefreshToken(string token, string ipAddress, string userAgent, int expirationInDay)
    {
        HashedToken = token;
        RemoteIp = ipAddress;
        UserAgent = userAgent;
        IsRevoked = false;


        var datetime = DateTime.UtcNow;
        CreateAt = datetime;
        ExpireAt = datetime.AddDays(expirationInDay);
    }


    // TODO: باید برای امنیت بیشتر به صورت هش شده دیتابیس ثبت شود
    // SHA256 => با این الگوریتم مناسب است
    // بهتره از سالت هم برای هش کردن استفاده شود
    public string HashedToken { get; private set; }
    //public string UserCode { get; private set; }
    public string RemoteIp { get; private set; }
    public string UserAgent { get; private set; }
    public DateTime CreateAt { get; private set; }
    public DateTime? ExpireAt { get; private set; }
    public bool IsExpire => DateTime.UtcNow > ExpireAt;
    public bool IsRevoked { get; private set; }
    public string RevokedByRemoteIp { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string RevokeReason { get; private set; } = string.Empty;
    public string? ReplacedByToken { get; private set; } = null;
    public bool IsActive => (ExpireAt is null || !IsExpire) && !IsRevoked;

    // TODO: زمانی که فرد لاگ اوت می کند، رفرش توکن آن باطل می شود
    // IsRevoked = true
    // براساس آیپی و یوزر ایجنت آن

    public static RefreshToken Construct(string token, string userIp, string userAgent, int expirationInDay)
        => new(token, userIp, userAgent, expirationInDay);

    public void ExpireIt()
        => ExpireAt = DateTime.UtcNow;

    public void ReplacedBy(string token)
        => ReplacedByToken = token;

    public void Revoked(string revokedById = "", string reason = "")
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedByRemoteIp = revokedById;
        RevokeReason = reason;
    }
}