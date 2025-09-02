namespace Parseh.Server.APIs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using global::Carter;
using Core.Contract.AppService.Command;

public sealed class UserAPIs : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users");

        group.MapPost("login", async ([FromBody] UserLoginCommand command, [FromServices] Responser requestController, CancellationToken token = default) =>
        {
            var response = await requestController.SendAsync<UserLoginCommand, LoginResponse>(command, token);
            return response.JsonOutput();
        })
        .WithName("/login");

        group.MapPost("refreshtoken", async ([FromBody] UserRefereshTokenCommand command, [FromServices] Responser requestController, CancellationToken token = default) =>
        {
            var response = await requestController.SendAsync<UserRefereshTokenCommand, LoginResponse>(command, token);
            return response.JsonOutput();
        })
        .WithName("/refreshtoken");

        group.MapGet("test", () =>
        {
            throw new Exception("This method is not implemented yet. Please implement the logic for user login.");
        });
    }
}
