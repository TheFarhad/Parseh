namespace Parseh.Server.Core.Contract.AppService.Command;

public sealed record UserLoginCommand(string UserName, string Password) : ICommandRequest<LoginResponse> { }
public sealed record UserRefereshTokenCommand(string UserCode) : ICommandRequest<RefreshTokenResponse> { }
