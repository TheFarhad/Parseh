namespace Parseh.Server.Core.Contract.Infra.Persistence.Command;

public interface ITokenService
{
    Task<Response<TokenResponse>> GenerateAccessTokenAsync(Domain.Aggregates.User.Entity.User user, CancellationToken cancellationToken);
    Task<Response<TokenResponse>> GenerateRefreshTokenAsync(Domain.Aggregates.User.Entity.User user, CancellationToken cancellationToken);
}