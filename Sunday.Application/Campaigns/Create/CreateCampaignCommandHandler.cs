using DotResults;
using Sunday.Application.Abstract;
using Sunday.Core.Abstract;
using Sunday.Core.Models;

namespace Sunday.Application.Campaigns.Create;

public class CreateCampaignCommandHandler : ICommandHandler<CreateCampaignCommand, string>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCampaignCommandHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(CreateCampaignCommand command, CancellationToken cancellationToken = default)
    {
        var campaign = new Campaign
        {
            Id = Guid.NewGuid().ToString(),
            Name = command.Name,
            BrandId = command.BrandId
        };

        await _clientRepository.CreateCampaignAsync(campaign, cancellationToken);
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        
        return result.IsFailure ? Result.Failure<string>(result.Error) : Result.Success(campaign.Id);
    }
}
