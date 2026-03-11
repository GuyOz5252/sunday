using Sunday.Application.Users.Register;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Sunday.Api.Groups;

namespace Sunday.Api.Endpoints.Users;

public class RegisterUserEndpoint : Endpoint<RegisterUserRequest, RegisterUserResponse>
{
    public required Sunday.Application.Abstract.ICommandHandler<RegisterUserCommand, string> CommandHandler { get; init; }
    
    public override void Configure()
    {
        Post("/register");
        Group<UsersEndpointsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterUserRequest request, CancellationToken ct)
    {
        var command = new RegisterUserCommand
        {
            Email = request.Email,
            UserName = request.UserName,
            Password = request.Password
        };
        var result = await CommandHandler.HandleAsync(command, ct);
        if (result.IsFailure)
        {
            await Send.ResultAsync(Results.BadRequest(result.Error));
        }
        await Send.OkAsync(new RegisterUserResponse(result.Value), ct);
    }
}

public record RegisterUserRequest(string UserName, string Email, string Password);

public record RegisterUserResponse(string UserId);
