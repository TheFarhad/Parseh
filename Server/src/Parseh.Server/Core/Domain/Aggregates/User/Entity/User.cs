namespace Parseh.Server.Core.Domain.Aggregates.User.Entity;

using System.Linq;
using Framework;
using Parseh.Server.Core.Domain.Aggregates.Role.ValueObject;
using ValueObject;

public sealed class User : AggregateRoot<UserId>
{
    public const string RoleBackingField = nameof(_roles);
    public const string RefreshTokenBackingField = nameof(_refereshTokens);

    private readonly List<UserRole> _roles = [];
    private readonly List<RefreshToken> _refereshTokens = [];

    private User() { }
    private User(string name, string family, string email, string username, string password)
    {
        Name = name;
        Family = family;
        Email = email;
        UserName = username;
        Password = password;
    }

    public string Name { get; private set; }
    public string Family { get; private set; }
    public string FullName => $"{Name} {Family}";
    public string Email { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();
    public IReadOnlyList<RefreshToken> RefreshTokens => _refereshTokens.AsReadOnly();

    public static User Construct(string name, string family, string email, string username, string password)
        => new(name, family, email, username, password);

    public void AssignRoles(params UserRole[] roles)
    {
        foreach (var role in roles)
        {
            if (_roles.Any(_ => _.RoleId == role.RoleId))
            {
                // TODO: throw new Exception("Role already exists for this user.");
            }
            else
                _roles.Add(UserRole.Construct(UserId.Construct(Id.Id), role.RoleId));
        }
    }

    public void RemoveRole(RoleId roleId)
    {

    }

    public void AddRerereshToken(RefreshToken token)
    {
        if (!_refereshTokens.Any(_ => _.HashedToken.Equals(token.HashedToken, StringComparison.Ordinal)))
            _refereshTokens.Add(token);

        else
        {
            // TODO: 
        }
    }
}
