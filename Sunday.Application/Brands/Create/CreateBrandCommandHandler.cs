using DotResults;
using Sunday.Application.Abstracts;
using Sunday.Core.Abstracts;
using Sunday.Core.DomainEvents;
using Sunday.Core.Models;

namespace Sunday.Application.Brands.Create;

public class CreateBrandCommandHandler : ICommandHandler<CreateBrandCommand, string>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBrandCommandHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(CreateBrandCommand command, CancellationToken cancellationToken = default)
    {
        var brand = new Brand
        {
            Id = Guid.NewGuid().ToString(),
            Name = command.Name,
            ClientId = command.ClientId
        };

        await _clientRepository.CreateBrandAsync(brand, cancellationToken);
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        
        return result.IsFailure ? Result.Failure<string>(result.Error) : Result.Success(brand.Id);
    }
}
