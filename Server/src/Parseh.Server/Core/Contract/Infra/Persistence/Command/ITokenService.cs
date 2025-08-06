namespace Parseh.Server.Core.Contract.Infra.Persistence.Command;

public interface ITokenService
{
    Task<Response<TokenResponse>> GenerateAccessTokenAsync(Domain.Aggregates.User.Entity.User user);
    Task<Response<TokenResponse>> GenerateRefereshTokenToken(Domain.Aggregates.User.Entity.User user);
}