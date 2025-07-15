namespace Parseh.Server.Core.Domain.Aggregates.User.Entity;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using ValueObject;

public sealed class User : AggregateRoot<UserId>
{
    public string Name { get; private set; }
    public string Family { get; private set; }
    public string FullName => $"{Name} {Family}";
    public string Email { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }

    private readonly List<UserRole> _roles = [];
    public IReadOnlyList<UserRole> UserRoles => _roles.AsReadOnly();

    private readonly List<RefreshToken> _refereshTokens = [];
    public IReadOnlyList<RefreshToken> RefereshTokens => _refereshTokens.AsReadOnly();

    private User() { }
    private User(string name, string family, string email, string username, string password)
    {
        Name = name;
        Family = family;
        Email = email;
        UserName = username;
        Password = password;
    }

    public static User New(string name, string family, string email, string username, string password)
        => new(name, family, email, username, password);

    public void AddRoles(params UserRole[] roles)
    {
        foreach (var role in roles)
        {
            if (_roles.Any(_ => _.RoleId == role.RoleId))
            {
                // TODO: throw new Exception("Role already exists for this user.");
            }
            else
                _roles.Add(UserRole.New(Id.Id, role.RoleId));
        }
    }

    public void AddRerereshToken(RefreshToken refereshToken)
    {
        if (_refereshTokens.Any(_ => _.Token.Equals(refereshToken.Token, StringComparison.Ordinal)))
            _refereshTokens.Add(refereshToken);

        else
        {
            // TODO: 
        }
    }
}

public sealed class RefreshToken : Entity<RefreshTokenId>
{
    public string Token { get; private set; }
    public string UserCode { get; private set; }
    public string UserIp { get; private set; }
    public string UserAgent { get; private set; }
    public DateTime CreateAt { get; private set; }
    public DateTime ExpireAt { get; private set; }
    public bool IsExpire => DateTime.UtcNow > ExpireAt;
    public bool IsRevoke { get; private set; }
    public string? ReplacedByToken { get; set; }


    // TODO: آیا به پراپرتی یوزر هم نیاز است


    // TODO: زمانی که فرد لاگ اوت می کند، رفرش توکن آن باطل می شود
    // براساس آیپی و یوزر ایجنت آن
    private RefreshToken() { }
    private RefreshToken(string token, string userCode, string userIp, string userAgent, int expirationInDay)
    {
        Token = token;
        UserCode = userCode;
        UserIp = userIp;
        UserAgent = userAgent;
        IsRevoke = false;


        var datetime = DateTime.UtcNow;
        CreateAt = datetime;
        ExpireAt = datetime.AddDays(expirationInDay);
    }



    public static RefreshToken New(string token, string userCode, string userIp, string userAgent, int expirationInDay)
        => new(token, userCode, userIp, userAgent, expirationInDay);

    public void ExpireIt() => ExpireAt = DateTime.UtcNow;
    public void Revoked() => IsRevoke = true;
}

public sealed class Role : AggregateRoot<UserId>
{
    public string Title { get; private set; }

    private readonly List<UserRole> _users = [];
    public IReadOnlyList<UserRole> Users => _users.AsReadOnly();

    private readonly List<RolePermission> _permissions = [];
    public IReadOnlyList<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role() { }
    private Role(string title)
    {
        Title = title;
    }

    public static Role New(string title)
        => new(title);
}

public sealed class UserRole
{
    public long UserId { get; private set; }
    public long RoleId { get; private set; }

    public User User { get; private set; }
    public Role Role { get; private set; }

    private UserRole() { }
    private UserRole(long userId, long roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public static UserRole New(long userId, long roleId)
        => new(userId, roleId);
}

public sealed class Permission : Entity<PermissionId>
{
    public string Title { get; private set; }

    private Permission() { }
    private Permission(string title)
    {
        Title = title;
    }

    public static Permission New(string title)
        => new(title);
}

public sealed class RolePermission
{
    public long RoleId { get; private set; }
    public long PermisssionId { get; private set; }

    private RolePermission() { }
    private RolePermission(long roleId, long permissionId)
    {
        RoleId = roleId;
        PermisssionId = permissionId;
    }
    public static RolePermission New(long roleId, long permissionId)
        => new(roleId, permissionId);
}



// ---------------------------------------------

public sealed record UserLoginCommand(string Username, string Password) : IRequest<TokenResponse> { }
public sealed record UserRefereshTokenCommand(string Token, string User) : IRequest<TokenResponse> { }

public sealed record TokenResponse(string Token, string RefereshToken, string User, DateTimeOffset TokenExpireDate, string RefereshokenExpireDate);


// ---------------------------------------------

public record JwtOption(string Issuer, string Audience, string SecretKey, int TokenExpirationInMin, int RefreshTokenExpirationInDay);


public class TokenBlacklist
{
    public int Id { get; set; }
    public string Jti { get; set; } = String.Empty;
    public DateTime ExpireAt { get; set; }
}


public class RsaKeyProvider
{
    public static RSA LoadPrivateKey(string path)
    {
        var rsa = RSA.Create();
        var keyText = File.ReadAllText(path);

        var fileInfo = new FileInfo(path);
        if ((fileInfo.Attributes & FileAttributes.ReadOnly) == 0)
            throw new UnauthorizedAccessException("Private key should be read-only!");

        rsa.ImportFromPem(keyText);
        return rsa;
    }

    public static RSA LoadPublicKey(string path)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(path));
        return rsa;
    }
}


// in token generator service:
//var rsaPrivate = RsaKeyProvider.LoadPrivateKey("Endpoint/Keys/Private/private.key");
// var accessToken = JwtTokenHelper.CreateJwt(claims, config, rsaPrivate);


