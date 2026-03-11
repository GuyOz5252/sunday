using DotResults;
using Sunday.Application.Abstract;
using Sunday.Core.Abstract;

namespace Sunday.Application.Users.Login;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IAuthService _authService;

    public LoginUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<LoginUserResult>> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        return await _authService.LoginUserAsync(command.Email, command.Password, cancellationToken);
    }
}
