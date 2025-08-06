namespace Parseh.Server.Core.Domain.Aggregates.User.Entity;

using Framework;
using ValueObject;

public sealed class RefreshToken : Entity<RefreshTokenId>
{
    // TODO: باید برای امنیت بیشتر به صورت هش شده دیتابیس ثبت شود
    // SHA256 => با این الگوریتم مناسب است
    // بهتره از سالت هم برای هش کردن استفاده شود
    public string HashedToken { get; private set; }
    //public string UserCode { get; private set; }
    public string RemoteIp { get; private set; }
    public string UserAgent { get; private set; }
    public DateTime CreateAt { get; private set; }
    public DateTime ExpireAt { get; private set; }
    public bool IsExpire => DateTime.UtcNow > ExpireAt;
    public bool IsRevoked { get; private set; }
    public string RevokedByRemoteIp { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string RevokeReason { get; private set; } = string.Empty;
    public string? ReplacedByToken { get; private set; } = null;

    // TODO: زمانی که فرد لاگ اوت می کند، رفرش توکن آن باطل می شود
    // IsRevoked = true
    // براساس آیپی و یوزر ایجنت آن

    RefreshToken() { }
    RefreshToken(string token, /*string userCode,*/ string ipAddress, string userAgent, int expirationInDay)
    {
        HashedToken = token;
        //UserCode = userCode;
        RemoteIp = ipAddress;
        UserAgent = userAgent;
        IsRevoked = false;


        var datetime = DateTime.UtcNow;
        CreateAt = datetime;
        ExpireAt = datetime.AddDays(expirationInDay);
    }

    public static RefreshToken New(string token, /*string userCode,*/ string userIp, string userAgent, int expirationInDay)
    => new(token, /*userCode,*/ userIp, userAgent, expirationInDay);

    public void ExpireIt() => ExpireAt = DateTime.UtcNow;
    public void TokenReplacedBy(string refreshToken) => ReplacedByToken = refreshToken;
    public void Revoked(string revokedById = "", string reason = "")
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedByRemoteIp = revokedById;
        RevokeReason = reason;
    }
}