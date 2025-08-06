namespace Parseh.Server.Core.AppService.Command;

using Contract.AppService.Command;
using Contract.Infra.Persistence.Command;
using Contract.Infra.Persistence.Command.User;
using Parseh.Server.Core.Domain.Aggregates.User.Entity;

public sealed class UserLoginCommandHandler(IUserCommandRepository userCommandRepository, ITokenService tokenService, IEncryptService encryptService, IParsehUnitOfWork unitOfWork)
    : CommandRequestHandler<UserLoginCommand, TokenResponse>(unitOfWork)
{
    readonly IUserCommandRepository _userCommandRepository = userCommandRepository;
    readonly ITokenService _tokenService = tokenService;
    readonly IEncryptService _encryptService = encryptService;

    public override async Task<Response<TokenResponse>> HandleAsync(UserLoginCommand command, CancellationToken token = default)
    {
        Response<TokenResponse> result = default!;
        try
        {
            var user = await _userCommandRepository
                                .SingleOrDefaultAsync(_ => _.UserName == command.Username);
            if (user is { })
            {
                var isCorrectPassword = _encryptService.Verify(command.Password, user.Password, user.Salt);
                if (!isCorrectPassword)
                {
                    result = Error.BadRequest("");
                }
                else
                {
                    result = await _tokenService.GenerateAccessTokenAsync(user);
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
    private readonly IUserCommandRepository _userCommandRepository = userCommandRepository;
    private readonly ITokenService _tokenService = tokenService;

    public override async Task<Response<TokenResponse>> HandleAsync(UserRefereshTokenCommand command, CancellationToken token = default)
    {
        Response<TokenResponse> result = default!;
        try
        {
            var user = await _userCommandRepository
                                .SingleOrDefaultAsync(
                                        [_ => _.RefreshTokens],
                                        _ => _.Code == Code.New(command.UserCode));

            if (user is { })
                result = await _tokenService.GenerateRefereshTokenToken(user);
            else
                result = Error.BadRequest("");
        }
        catch (Exception e)
        {
            result = Error.Unauthorized(e.Message);
        }
        return result;
    }
}