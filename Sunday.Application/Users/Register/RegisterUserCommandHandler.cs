using DotResults;
using Sunday.Application.Abstract;

namespace Sunday.Application.Users.Register;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, string>
{
    public Task<Result<string>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
