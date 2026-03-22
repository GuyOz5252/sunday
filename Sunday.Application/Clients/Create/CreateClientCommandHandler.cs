using DotResults;
using Sunday.Application.Abstract;
using Sunday.Core.Abstract;
using Sunday.Core.Models;

namespace Sunday.Application.Clients.Create;

public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, string>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClientCommandHandler(
        IClientRepository clientRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(CreateClientCommand command, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(command.AccountManagerId))
        {
            var accountManagerResult = await _userRepository.GetUserAsync(command.AccountManagerId);
            if (accountManagerResult.IsFailure)
            {
                return Result.Failure<string>(accountManagerResult.Error);
            }

            var accountManager = accountManagerResult.Value;
            if (accountManager.AgencyId != command.AgencyId)
            {
                return Result.Failure<string>(new Error("Client.InvalidAccountManager", "Account manager does not belong to the agency", "BadRequest"));
            }

            if (!accountManager.Roles.Contains(Role.AccountManager))
            {
                return Result.Failure<string>(new Error("Client.InvalidAccountManager", "User is not an account manager", "BadRequest"));
            }
        }

        var client = new Client
        {
            Id = Guid.NewGuid().ToString(),
            Name = command.Name,
            AgencyId = command.AgencyId,
            AccountManagerId = command.AccountManagerId
        };

        await _clientRepository.CreateClientAsync(client, cancellationToken);
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        
        return result.IsFailure ? Result.Failure<string>(result.Error) : Result.Success(client.Id);
    }
}
