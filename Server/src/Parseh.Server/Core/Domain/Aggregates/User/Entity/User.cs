namespace Parseh.Server.Core.Domain.Aggregates.User.Entity;

using System.Linq;
using Framework;
using ValueObject;

public sealed class User : AggregateRoot<UserId>
{
    public const string RoleBackingField = nameof(_roles);
    public const string RefreshTokenBackingField = nameof(_refereshTokens);

    readonly List<UserRole> _roles = [];
    readonly List<RefreshToken> _refereshTokens = [];

    public string Name { get; private set; }
    public string Family { get; private set; }
    public string FullName => $"{Name} {Family}";
    public string Email { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public string Salt { get; private set; }

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();
    public IReadOnlyList<RefreshToken> RefreshTokens => _refereshTokens.AsReadOnly();

    private User() { }
    private User(string name, string family, string email, string username, string password, string salt)
    {
        Name = name;
        Family = family;
        Email = email;
        UserName = username;
        Password = password;
        Salt = salt;
    }

    public static User New(string name, string family, string email, string username, string password, string salt)
        => new(name, family, email, username, password, salt);

    public void AssignRoles(params UserRole[] roles)
    {
        foreach (var role in roles)
        {
            if (_roles.Any(_ => _.RoleId == role.RoleId))
            {
                // TODO: throw new Exception("Role already exists for this user.");
            }
            else
                _roles.Add(UserRole.New(UserId.New(Id.Id), role.RoleId));
        }
    }

    public void RemoveRole(long roleId)
    {

    }

    public void AddRerereshToken(RefreshToken refereshToken)
    {
        if (!_refereshTokens.Any(_ => _.HashedToken.Equals(refereshToken.HashedToken, StringComparison.Ordinal)))
            _refereshTokens.Add(refereshToken);

        else
        {
            // TODO: 
        }
    }
}
