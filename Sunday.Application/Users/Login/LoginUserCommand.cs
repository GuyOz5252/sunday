using Sunday.Application.Abstract;
using Sunday.Core.Abstract;

namespace Sunday.Application.Users.Login;

public record LoginUserCommand : ICommand<LoginUserResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
