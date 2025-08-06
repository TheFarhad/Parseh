namespace Parseh.Server.Core.Contract.AppService.Command;

public sealed record UserLoginCommand(string Username, string Password) : ICommandRequest<TokenResponse> { }
public sealed record UserRefereshTokenCommand(string UserCode) : ICommandRequest<TokenResponse> { }
