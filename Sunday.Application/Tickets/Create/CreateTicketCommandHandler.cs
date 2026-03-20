using DotResults;
using Sunday.Application.Abstracts;
using Sunday.Core.Abstracts;
using Sunday.Core.Models;

namespace Sunday.Application.Tickets.Create;

public class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, string>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        IClientRepository clientRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _clientRepository = clientRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(CreateTicketCommand command, CancellationToken cancellationToken = default)
    {
        var clientResult = await _clientRepository.GetAsync(command.ClientId, cancellationToken);
        if (clientResult.IsFailure)
        {
            return Result.Failure<string>(clientResult.Error);
        }
        
        if (clientResult.Value.AgencyId != command.AgencyId)
        {
            return Result.Failure<string>(new Error("Ticket.InvalidClient", "Client does not belong to the agency", "BadRequest"));
        }

        var creatorResult = await _userRepository.GetUserAsync(command.CreatorUserId);
        if (creatorResult.IsFailure)
        {
            return Result.Failure<string>(creatorResult.Error);
        }

        if (creatorResult.Value.AgencyId != command.AgencyId)
        {
            return Result.Failure<string>(new Error("Ticket.InvalidCreator", "Creator does not belong to the agency", "BadRequest"));
        }

        if (!string.IsNullOrEmpty(command.BrandId))
        {
            var brandResult = await _clientRepository.GetBrandAsync(command.BrandId, cancellationToken);
            if (brandResult.IsFailure)
            {
                return Result.Failure<string>(brandResult.Error);
            }
            if (brandResult.Value.ClientId != command.ClientId)
            {
                return Result.Failure<string>(new Error("Ticket.InvalidBrand", "Brand does not belong to the client", "BadRequest"));
            }
        }

        if (!string.IsNullOrEmpty(command.CampaignId))
        {
            var campaignResult = await _clientRepository.GetCampaignAsync(command.CampaignId, cancellationToken);
            if (campaignResult.IsFailure)
            {
                return Result.Failure<string>(campaignResult.Error);
            }
            if (campaignResult.Value.BrandId != command.BrandId)
            {
                return Result.Failure<string>(new Error("Ticket.InvalidCampaign", "Campaign does not belong to the brand", "BadRequest"));
            }
        }

        var ticket = new Ticket
        {
            Id = Guid.NewGuid().ToString(),
            AgencyId = command.AgencyId,
            ClientId = command.ClientId,
            BrandId = command.BrandId,
            CampaignId = command.CampaignId,
            Title = command.Title,
            Brief = command.Brief,
            Status = TicketStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            DueDate = command.DueDate
        };

        ticket.Watchers.Add(new TicketWatcher
        {
            TicketId = ticket.Id,
            UserId = command.CreatorUserId
        });

        if (!string.IsNullOrWhiteSpace(clientResult.Value.AccountManagerId) &&
            clientResult.Value.AccountManagerId != command.CreatorUserId)
        {
            ticket.Watchers.Add(new TicketWatcher
            {
                TicketId = ticket.Id,
                UserId = clientResult.Value.AccountManagerId
            });
        }

        await _ticketRepository.CreateTicketAsync(ticket, cancellationToken);
        var commitResult = await _unitOfWork.CommitAsync(cancellationToken);
        
        return commitResult.IsFailure ? Result.Failure<string>(commitResult.Error) : Result.Success(ticket.Id);
    }
}
