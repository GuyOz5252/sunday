using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Sunday.Api.Groups;
using Sunday.Application.Users.Login;

namespace Sunday.Api.Endpoints.Users;

public class LoginUserEndpoint : Endpoint<LoginUserRequest, LoginUserResponse>
{
    public required LoginUserCommandHandler CommandHandler { get; init; }

    public override void Configure()
    {
        Post("/login");
        Group<UsersEndpointsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginUserRequest request, CancellationToken ct)
    {
        var command = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };
        var result = await CommandHandler.HandleAsync(command, ct);
        if (result.IsFailure)
        {
            await Send.ResultAsync(Results.BadRequest(result.Error));
        }

        await Send.OkAsync(new LoginUserResponse(
                result.Value.UserId,
                result.Value.Token,
                result.Value.Roles),
            ct);
    }
}

public record LoginUserRequest(string Email, string Password);

public record LoginUserResponse(string UserId, string Token, List<string> Roles);
