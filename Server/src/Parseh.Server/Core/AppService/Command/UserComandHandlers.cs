namespace Parseh.Server.Core.AppService.Command;

using Contract.AppService.Command;
using Contract.Infra.Persistence.Command;
using Contract.Infra.Persistence.Command.User;

public sealed class UserLoginCommandHandler(IUserCommandRepository userCommandRepository, ITokenService tokenService, IEncryptionService encryptService, IParsehUnitOfWork unitOfWork)
    : CommandRequestHandler<UserLoginCommand, LoginResponse>(unitOfWork)
{
    readonly IUserCommandRepository _userCommandRepository = userCommandRepository;
    readonly ITokenService _tokenService = tokenService;
    readonly IEncryptionService _encryptService = encryptService;

    public override async Task<Response<LoginResponse>> HandleAsync(UserLoginCommand command, CancellationToken cancellationToken = default)
    {
        Response<LoginResponse> result = default!;
        try
        {
            if (command is null)
            {
                return Error.BadRequest("");
            }
            if (command.UserName.IsEmpty() is true || command.Password.IsEmpty() is true)
            {
                return Error.BadRequest("");
            }

            // TODO: رول ها و پرمیژن های یوزر هم واکشی شود
            // آیا باید اطلاعات را اینجا واکشی کنیم یا بهتر است در درخواست های جداگانه، در توکن سرویس اینکار انجام شود

            List<string> includes = ["Roles", "Roles.Role", "Roles.Role.Cliams", "Roles.Role.Cliams.Cliam"];
            var user = await _userCommandRepository
                                .SingleOrDefaultAsync(includes, _ => _.UserName == command.UserName, cancellationToken);

            if (user is { })
            {
                var isCorrectPassword = _encryptService.Verify(command.Password, user.Password);
                if (isCorrectPassword)
                {
                    result = await _tokenService.GenerateAccessTokenAsync(user, cancellationToken);
                }
                else
                {
                    result = Error.BadRequest("");
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
    : CommandRequestHandler<UserRefereshTokenCommand, RefreshTokenResponse>(unitOfWork)
{
    readonly IUserCommandRepository _userCommandRepository = userCommandRepository;
    readonly ITokenService _tokenService = tokenService;

    public override async Task<Response<RefreshTokenResponse>> HandleAsync(UserRefereshTokenCommand command, CancellationToken canellationToken = default)
    {
        Response<RefreshTokenResponse> result = default!;
        try
        {
            List<string> includes = ["RefreshTokens", "Roles", "Roles.Role", "Roles.Role.Cliams", "Roles.Role.Cliams.Cliam"];
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