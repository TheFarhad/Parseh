namespace Parseh.Server.Core.Contract.Infra.Persistence.Command;

public interface ITokenService
{
    Task<Response<LoginResponse>> GenerateAccessTokenAsync(Domain.Aggregates.User.Entity.User user, CancellationToken cancellationToken);
    Task<Response<RefreshTokenResponse>> GenerateRefreshTokenAsync(Domain.Aggregates.User.Entity.User user, CancellationToken cancellationToken);
}