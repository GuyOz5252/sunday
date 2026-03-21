using DotResults;
using Sunday.Application.Abstracts;
using Sunday.Core.Abstracts;

namespace Sunday.Application.WorkSessions.Stop;

public class StopWorkSessionCommandHandler : ICommandHandler<StopWorkSessionCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StopWorkSessionCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(StopWorkSessionCommand command, CancellationToken cancellationToken = default)
    {
        var activeSessionResult = await _ticketRepository.GetActiveWorkSessionAsync(command.UserId, cancellationToken);
        if (activeSessionResult.IsFailure)
        {
            return Result.Failure(activeSessionResult.Error);
        }

        var session = activeSessionResult.Value;
        session.EndTime = DateTime.UtcNow;
        session.Description = command.Description ?? session.Description;

        return await _unitOfWork.CommitAsync(cancellationToken);
    }
}
