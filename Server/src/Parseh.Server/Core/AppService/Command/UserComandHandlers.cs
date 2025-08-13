namespace Parseh.Server.Core.AppService.Command;

using Contract.AppService.Command;
using Contract.Infra.Persistence.Command;
using Contract.Infra.Persistence.Command.User;

public sealed class UserLoginCommandHandler(IUserCommandRepository userCommandRepository, ITokenService tokenService, IEncryptService encryptService, IParsehUnitOfWork unitOfWork)
    : CommandRequestHandler<UserLoginCommand, TokenResponse>(unitOfWork)
{
    readonly IUserCommandRepository _userCommandRepository = userCommandRepository;
    readonly ITokenService _tokenService = tokenService;
    readonly IEncryptService _encryptService = encryptService;

    public override async Task<Response<TokenResponse>> HandleAsync(UserLoginCommand command, CancellationToken cancellationToken = default)
    {
        Response<TokenResponse> result = default!;
        try
        {
            // TODO: رول ها و پرمیژن های یوزر هم واکشی شود
            // آیا باید اطلاعات را اینجا واکشی کنیم یا بهتر است در درخواست های جداگانه، در توکن سرویس اینکار انجام شود

            List<string> includes = ["Roles", "Roles.Role", "Roles.Role.Permissions", "Roles.Role.Permissions.Permission"];
            var user = await _userCommandRepository
                                .SingleOrDefaultAsync(includes, _ => _.UserName == command.UserName, cancellationToken);

            if (user is { })
            {
                var isCorrectPassword = _encryptService.Verify(command.Password, user.Password, user.Salt);
                if (isCorrectPassword)
                {
                    result = await _tokenService.GenerateAccessTokenAsync(user, cancellationToken);
                }
            }
            else
            {
                result = Error.BadRequest("");
            }
        }
        catch (Exception e)
        {
            result = Error.Unauthorized(e.Message);
        }
        return result;
    }
}

public sealed class UserRefreshTokenCommandHandler(IUserCommandRepository userCommandRepository, ITokenService tokenService, IParsehUnitOfWork unitOfWork)
    : CommandRequestHandler<UserRefereshTokenCommand, TokenResponse>(unitOfWork)
{
    readonly IUserCommandRepository _userCommandRepository = userCommandRepository;
    readonly ITokenService _tokenService = tokenService;

    public override async Task<Response<TokenResponse>> HandleAsync(UserRefereshTokenCommand command, CancellationToken canellationToken = default)
    {
        Response<TokenResponse> result = default!;
        try
        {
            List<string> includes = ["RefreshTokens", "Roles", "Roles.Role", "Roles.Role.Permissions", "Roles.Role.Permissions.Permission"];
            var user = await _userCommandRepository
                                .SingleOrDefaultAsync(includes, _ => _.Code == Code.New(command.UserCode));

            if (user is { })
            {
                result = await _tokenService.GenerateRefreshTokenAsync(user, canellationToken);
            }
            else
            {
                result = Error.BadRequest("");
            }
        }
        catch (Exception e)
        {
            result = Error.Unauthorized(e.Message);
        }
        return result;
    }
}