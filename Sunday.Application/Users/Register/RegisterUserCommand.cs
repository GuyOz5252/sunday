using Sunday.Application.Abstract;

namespace Sunday.Application.Users.Register;

public record RegisterUserCommand : ICommand<string>
{
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
}
