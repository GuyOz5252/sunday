using DotResults;
using Sunday.Application.Abstract;
using Sunday.Core.Abstract;
using Sunday.Core.Models;

namespace Sunday.Application.Tickets.Create;

public class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, string>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IAgencyRepository _agencyRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        IClientRepository clientRepository,
        IAgencyRepository agencyRepository,
        IBoardRepository boardRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _clientRepository = clientRepository;
        _agencyRepository = agencyRepository;
        _boardRepository = boardRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(CreateTicketCommand command, CancellationToken cancellationToken = default)
    {
        var campaignResult = await _clientRepository.GetCampaignAsync(command.CampaignId, cancellationToken);
        if (campaignResult.IsFailure)
        {
            return Result.Failure<string>(campaignResult.Error);
        }

        var campaign = campaignResult.Value;
        var clientId = campaign.Brand.ClientId;

        var clientResult = await _clientRepository.GetAsync(clientId, cancellationToken);
        if (clientResult.IsFailure)
        {
            return Result.Failure<string>(clientResult.Error);
        }

        if (clientResult.Value.AgencyId != command.AgencyId)
        {
            return Result.Failure<string>(new Error("Ticket.InvalidCampaign", "Campaign does not belong to this agency", "BadRequest"));
        }

        var agencyResult = await _agencyRepository.GetAsync(command.AgencyId, cancellationToken);
        if (agencyResult.IsFailure)
        {
            return Result.Failure<string>(agencyResult.Error);
        }

        var boardResult = await _boardRepository.GetAsync(command.BoardId, cancellationToken);
        if (boardResult.IsFailure)
        {
            return Result.Failure<string>(boardResult.Error);
        }

        if (boardResult.Value.BusinessUnitId != agencyResult.Value.BusinessUnitId)
        {
            return Result.Failure<string>(new Error("Ticket.InvalidBoard", "Board does not belong to this agency's business unit", "BadRequest"));
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

        var ticket = new Ticket
        {
            Id = Guid.NewGuid().ToString(),
            AgencyId = command.AgencyId,
            ClientId = clientId,
            CampaignId = command.CampaignId,
            BoardId = command.BoardId,
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
                UserId = clientResult.Value.AccountManagerId!
            });
        }

        await _ticketRepository.CreateTicketAsync(ticket, cancellationToken);
        var commitResult = await _unitOfWork.CommitAsync(cancellationToken);

        return commitResult.IsFailure ? Result.Failure<string>(commitResult.Error) : Result.Success(ticket.Id);
    }
}
