namespace Framework;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public interface IIdentityService
{
    string Id();
    string Ip();
    string FirstName();
    string LastName();
    string Username();
    string Agent();
    string Claim(string claimType);
    bool IsCurrentUser(string userId);
    string IdOrDefault();
}

public enum IdentityServiceFlag : byte { Origin = 0, Fake = 1 }

public sealed class IdentityService : IIdentityService
{
    private readonly HttpContext _context;
    private readonly IdentityServiceConfig _configs;

    public IdentityService(IHttpContextAccessor contextAccessor, IOptionsMonitor<IdentityServiceConfig> options)
    {
        if (contextAccessor is null || contextAccessor.HttpContext is null)
            throw new ArgumentNullException(nameof(contextAccessor));

        _context = contextAccessor.HttpContext;
        _configs = options.CurrentValue;
    }

    public string Id() => _context.GetClaim(ClaimTypes.NameIdentifier);
    public string Ip() => _context?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0";
    public string FirstName() => Claim(ClaimTypes.GivenName);
    public string LastName() => Claim(ClaimTypes.Surname);
    public string Username() => Claim(ClaimTypes.Name);
    public string Agent() => _context?.Request.Headers["User-Agent"] ?? "Unknown";
    public string Claim(string claimType) => _context.GetClaim(claimType);
    public string IdOrDefault() => Id() ?? _configs.DefaultId;
    public bool IsCurrentUser(string userId) => String.Equals(Id(), userId, StringComparison.OrdinalIgnoreCase);
}

public sealed class FakeIdentityService : IIdentityService
{
    private const string _defaultUserId = "1";

    public string FirstName() => "FirstName";
    public string LastName() => "LastName";
    public string Agent() => _defaultUserId;
    public string Id() => _defaultUserId;
    public string IdOrDefault() => _defaultUserId;
    public string IdOrDefault(string defaultValue) => defaultValue;
    public string Ip() => "0.0.0.0";
    public string Username() => "Username";
    public bool HasAccess(string access) => true;
    public string Claim(string claimType) => claimType;
    public bool IsCurrentUser(string userId) => true;
}

public sealed record IdentityServiceConfig
{
    public string DefaultId { get; set; } = String.Empty;
}
