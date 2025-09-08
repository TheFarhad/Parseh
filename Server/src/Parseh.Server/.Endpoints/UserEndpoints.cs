namespace Parseh.Server.APIs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Core.Contract.AppService.Command;
using Framework.Presentation;

public sealed class UserEndpoints : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users");

        group.MapPost("/login", async ([FromBody] UserLoginCommand command, [FromServices] Responser resposer, CancellationToken token = default) =>
        {
            var response = await resposer.SendAsync<UserLoginCommand, LoginResponse>(command, token);
            return response.JsonOutput();
        })
        .WithName("login");

        group.MapPost("/refreshtoken", async ([FromBody] UserRefereshTokenCommand command, [FromServices] Responser responser, CancellationToken token = default) =>
        {
            var response = await responser.SendAsync<UserRefereshTokenCommand, RefreshTokenResponse>(command, token);
            return response.JsonOutput();
        })
        .WithName("refreshtoken");

        group.MapGet("/test", () =>
        {
            throw new Exception("This method is not implemented yet. Please implement the logic for user login.");
        });
    }
}
