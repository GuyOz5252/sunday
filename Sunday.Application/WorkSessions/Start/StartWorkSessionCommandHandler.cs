using DotResults;
using Sunday.Application.Abstract;
using Sunday.Core.Abstract;
using Sunday.Core.Models;

namespace Sunday.Application.WorkSessions.Start;

public class StartWorkSessionCommandHandler : ICommandHandler<StartWorkSessionCommand, string>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartWorkSessionCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(StartWorkSessionCommand command, CancellationToken cancellationToken = default)
    {
        var activeSessionResult = await _ticketRepository.GetActiveWorkSessionAsync(command.UserId, cancellationToken);
        if (activeSessionResult.IsSuccess)
        {
            activeSessionResult.Value.EndTime = DateTime.UtcNow;
        }

        var ticketResult = await _ticketRepository.GetAsync(command.TicketId, cancellationToken);
        if (ticketResult.IsFailure)
        {
            return Result.Failure<string>(ticketResult.Error);
        }

        var session = new WorkSession
        {
            Id = Guid.NewGuid().ToString(),
            TicketId = command.TicketId,
            UserId = command.UserId,
            StartTime = DateTime.UtcNow,
            Description = command.Description
        };

        await _ticketRepository.CreateWorkSessionAsync(session, cancellationToken);
        
        var ticket = ticketResult.Value;
        if (ticket.Status == TicketStatus.AssignedToCopy)
        {
            ticket.Status = TicketStatus.InCopy;
        }
        else if (ticket.Status == TicketStatus.AssignedToDesigner)
        {
            ticket.Status = TicketStatus.InDesign;
        }

        var commitResult = await _unitOfWork.CommitAsync(cancellationToken);
        return commitResult.IsFailure ? Result.Failure<string>(commitResult.Error) : Result.Success(session.Id);
    }
}
