using DotResults;
using Sunday.Application.Abstracts;
using Sunday.Core.Abstracts;
using Sunday.Core.DomainEvents;
using Sunday.Core.Models;

namespace Sunday.Application.Tickets.UpdateStatus;

public class UpdateTicketStatusCommandHandler : ICommandHandler<UpdateTicketStatusCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTicketStatusCommandHandler(
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

    public async Task<Result> HandleAsync(UpdateTicketStatusCommand command, CancellationToken cancellationToken = default)
    {
        var ticketResult = await _ticketRepository.GetAsync(command.TicketId, cancellationToken);
        if (ticketResult.IsFailure)
        {
            return Result.Failure(ticketResult.Error);
        }

        var ticket = ticketResult.Value;
        var previousStatus = ticket.Status;

        var actorResult = await _userRepository.GetUserAsync(command.UserId);
        if (actorResult.IsFailure)
        {
            return Result.Failure(actorResult.Error);
        }

        var actor = actorResult.Value;
        if (actor.AgencyId != ticket.AgencyId)
        {
            return Result.Failure(new Error("Ticket.InvalidActor", "User does not belong to the ticket agency", "BadRequest"));
        }

        var result = ValidateTransition(ticket, command.NewStatus, command, actor.Roles);
        if (result.IsFailure)
        {
            return result;
        }

        ticket.Status = command.NewStatus;
        ticket.HandoffNote = command.Note ?? ticket.HandoffNote;

        if (command.NewStatus is TicketStatus.AssignedToCopy or TicketStatus.AssignedToDesigner)
        {
            var role = command.NewStatus == TicketStatus.AssignedToCopy ? Role.Copywriter : Role.Designer;
            foreach (var assignment in ticket.Assignments.Where(a => a.Role == role))
            {
                assignment.IsActive = false;
            }

            ticket.Assignments.Add(new TicketAssignment
            {
                Id = Guid.NewGuid().ToString(),
                TicketId = ticket.Id,
                UserId = command.NextAssigneeId!,
                Role = role,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            });

            if (ticket.Watchers.All(w => w.UserId != command.NextAssigneeId))
            {
                ticket.Watchers.Add(new TicketWatcher
                {
                    TicketId = ticket.Id,
                    UserId = command.NextAssigneeId!
                });
            }
        }

        await EnsureClientAccountManagerWatcherAsync(ticket, cancellationToken);

        if (IsWorkState(previousStatus) && !IsWorkState(command.NewStatus))
        {
            var activeSessionResult = await _ticketRepository.GetActiveWorkSessionAsync(command.UserId, cancellationToken);
            if (activeSessionResult.IsSuccess)
            {
                activeSessionResult.Value.EndTime = DateTime.UtcNow;
            }
        }

        if (ShouldIncrementRevision(previousStatus, command.NewStatus))
        {
            ticket.RevisionRound++;
        }

        return await _unitOfWork.CommitAsync(cancellationToken);
    }

    private Result ValidateTransition(Ticket ticket, TicketStatus nextStatus, UpdateTicketStatusCommand command, List<Role> actorRoles)
    {
        var roleResult = ValidateRole(ticket.Status, nextStatus, actorRoles);
        if (roleResult.IsFailure)
        {
            return roleResult;
        }

        return (ticket.Status, nextStatus) switch
        {
            (TicketStatus.Draft, TicketStatus.New) => Result.Success(),
            (TicketStatus.New, TicketStatus.Draft) => Result.Success(),
            (TicketStatus.New, TicketStatus.AwaitingTraffic) => Result.Success(),
            (TicketStatus.AwaitingTraffic, TicketStatus.AwaitingBriefCompletion) => Require(command.Note != null, "Note is required"),
            (TicketStatus.AwaitingTraffic, TicketStatus.AssignedToCopy) => Require(command.NextAssigneeId != null, "Assignee is required"),
            (TicketStatus.AwaitingTraffic, TicketStatus.AssignedToDesigner) => Require(command.NextAssigneeId != null, "Assignee is required"),
            (TicketStatus.AwaitingBriefCompletion, TicketStatus.AwaitingTraffic) => Result.Success(),
            (TicketStatus.AssignedToCopy, TicketStatus.InCopy) => Result.Success(),
            (TicketStatus.AssignedToCopy, TicketStatus.AwaitingTraffic) => Require(command.Note != null, "Note is required"),
            (TicketStatus.InCopy, TicketStatus.ReadyForDesign) => Result.Success(),
            (TicketStatus.InCopy, TicketStatus.AwaitingTraffic) => Require(command.Note != null, "Note is required"),
            (TicketStatus.ReadyForDesign, TicketStatus.AssignedToDesigner) => Require(command.NextAssigneeId != null, "Assignee is required"),
            (TicketStatus.AssignedToDesigner, TicketStatus.InDesign) => Result.Success(),
            (TicketStatus.AssignedToDesigner, TicketStatus.AwaitingTraffic) => Require(command.Note != null, "Note is required"),
            (TicketStatus.InDesign, TicketStatus.AwaitingCreativeApproval) => Result.Success(),
            (TicketStatus.InDesign, TicketStatus.AwaitingTraffic) => Require(command.Note != null, "Note is required"),
            (TicketStatus.AwaitingCreativeApproval, TicketStatus.ReturnToDesign) => Result.Success(),
            (TicketStatus.AwaitingCreativeApproval, TicketStatus.ReturnToCopy) => Result.Success(),
            (TicketStatus.AwaitingCreativeApproval, TicketStatus.AwaitingAmApproval) => Result.Success(),
            (TicketStatus.AwaitingCreativeApproval, TicketStatus.AwaitingBriefClarification) => Require(command.Note != null, "Note is required"),
            (TicketStatus.AwaitingBriefClarification, TicketStatus.AwaitingCreativeApproval) => Require(command.Note != null, "Note is required"),
            (TicketStatus.AwaitingAmApproval, TicketStatus.SentToClient) => Result.Success(),
            (TicketStatus.AwaitingAmApproval, TicketStatus.ReturnToDesign) => Require(command.Note != null, "Note is required"),
            (TicketStatus.AwaitingAmApproval, TicketStatus.ReturnToCopy) => Require(command.Note != null, "Note is required"),
            (TicketStatus.SentToClient, TicketStatus.AwaitingClientFeedback) => Result.Success(),
            (TicketStatus.SentToClient, TicketStatus.AwaitingAmApproval) => Require(command.Note != null, "Note is required"),
            (TicketStatus.AwaitingClientFeedback, TicketStatus.ReturnFromClient) => Result.Success(),
            (TicketStatus.ReturnToCopy, TicketStatus.AssignedToCopy) => Require(command.NextAssigneeId != null, "Assignee is required"),
            (TicketStatus.ReturnToCopy, TicketStatus.InCopy) => Result.Success(),
            (TicketStatus.ReturnToDesign, TicketStatus.AssignedToDesigner) => Require(command.NextAssigneeId != null, "Assignee is required"),
            (TicketStatus.ReturnToDesign, TicketStatus.InDesign) => Result.Success(),
            (TicketStatus.ReturnFromClient, TicketStatus.AssignedToCopy) => Require(command.NextAssigneeId != null, "Assignee is required"),
            (TicketStatus.ReturnFromClient, TicketStatus.AssignedToDesigner) => Require(command.NextAssigneeId != null, "Assignee is required"),
            (TicketStatus.ReturnFromClient, TicketStatus.AwaitingAmApproval) => Require(command.Note != null, "Note is required"),
            (TicketStatus.AwaitingClientFeedback, TicketStatus.Completed) => Result.Success(),
            (TicketStatus.Completed, TicketStatus.Closed) => Result.Success(),
            (TicketStatus.OnHold, TicketStatus.AwaitingTraffic) => Require(command.Note != null, "Note is required"),
            (_, TicketStatus.OnHold) => Result.Success(),
            (_, TicketStatus.Cancelled) => Result.Success(),
            _ => Result.Failure(new Error("Ticket.InvalidTransition", $"Transition from {ticket.Status} to {nextStatus} is not allowed", "BadRequest"))
        };
    }

    private Result Require(bool condition, string message)
    {
        return condition ? Result.Success() : Result.Failure(new Error("Ticket.ValidationError", message, "BadRequest"));
    }

    private Result ValidateRole(TicketStatus oldStatus, TicketStatus newStatus, List<Role> actorRoles)
    {
        var allowedRoles = GetAllowedRoles(oldStatus, newStatus);
        if (allowedRoles.Count == 0)
        {
            return Result.Success();
        }

        return actorRoles.Any(allowedRoles.Contains)
            ? Result.Success()
            : Result.Failure(new Error("Ticket.ForbiddenTransition", "User is not allowed to perform this transition", "Forbidden"));
    }

    private HashSet<Role> GetAllowedRoles(TicketStatus oldStatus, TicketStatus newStatus)
    {
        return (oldStatus, newStatus) switch
        {
            (TicketStatus.Draft, TicketStatus.New) => [Role.TrafficManager],
            (TicketStatus.New, TicketStatus.Draft) => [Role.TrafficManager],
            (TicketStatus.New, TicketStatus.AwaitingTraffic) => [Role.TrafficManager],
            (TicketStatus.AwaitingTraffic, TicketStatus.AwaitingBriefCompletion) => [Role.TrafficManager],
            (TicketStatus.AwaitingTraffic, TicketStatus.AssignedToCopy) => [Role.TrafficManager],
            (TicketStatus.AwaitingTraffic, TicketStatus.AssignedToDesigner) => [Role.TrafficManager],
            (TicketStatus.AwaitingBriefCompletion, TicketStatus.AwaitingTraffic) => [Role.AccountManager],
            (TicketStatus.AssignedToCopy, TicketStatus.InCopy) => [Role.Copywriter],
            (TicketStatus.AssignedToCopy, TicketStatus.AwaitingTraffic) => [Role.TrafficManager],
            (TicketStatus.InCopy, TicketStatus.ReadyForDesign) => [Role.Copywriter],
            (TicketStatus.InCopy, TicketStatus.AwaitingTraffic) => [Role.TrafficManager],
            (TicketStatus.ReadyForDesign, TicketStatus.AssignedToDesigner) => [Role.TrafficManager],
            (TicketStatus.AssignedToDesigner, TicketStatus.InDesign) => [Role.Designer],
            (TicketStatus.AssignedToDesigner, TicketStatus.AwaitingTraffic) => [Role.TrafficManager],
            (TicketStatus.InDesign, TicketStatus.AwaitingCreativeApproval) => [Role.Designer],
            (TicketStatus.InDesign, TicketStatus.AwaitingTraffic) => [Role.TrafficManager],
            (TicketStatus.AwaitingCreativeApproval, TicketStatus.ReturnToDesign) => [Role.CreativeManager],
            (TicketStatus.AwaitingCreativeApproval, TicketStatus.ReturnToCopy) => [Role.CreativeManager],
            (TicketStatus.AwaitingCreativeApproval, TicketStatus.AwaitingAmApproval) => [Role.CreativeManager],
            (TicketStatus.AwaitingCreativeApproval, TicketStatus.AwaitingBriefClarification) => [Role.CreativeManager],
            (TicketStatus.AwaitingBriefClarification, TicketStatus.AwaitingCreativeApproval) => [Role.AccountManager],
            (TicketStatus.AwaitingAmApproval, TicketStatus.SentToClient) => [Role.AccountManager],
            (TicketStatus.AwaitingAmApproval, TicketStatus.ReturnToDesign) => [Role.AccountManager],
            (TicketStatus.AwaitingAmApproval, TicketStatus.ReturnToCopy) => [Role.AccountManager],
            (TicketStatus.SentToClient, TicketStatus.AwaitingClientFeedback) => [Role.AccountManager],
            (TicketStatus.SentToClient, TicketStatus.AwaitingAmApproval) => [Role.AccountManager],
            (TicketStatus.AwaitingClientFeedback, TicketStatus.ReturnFromClient) => [Role.AccountManager],
            (TicketStatus.ReturnToCopy, TicketStatus.AssignedToCopy) => [Role.TrafficManager],
            (TicketStatus.ReturnToCopy, TicketStatus.InCopy) => [Role.Copywriter],
            (TicketStatus.ReturnToDesign, TicketStatus.AssignedToDesigner) => [Role.TrafficManager],
            (TicketStatus.ReturnToDesign, TicketStatus.InDesign) => [Role.Designer],
            (TicketStatus.ReturnFromClient, TicketStatus.AssignedToCopy) => [Role.TrafficManager],
            (TicketStatus.ReturnFromClient, TicketStatus.AssignedToDesigner) => [Role.TrafficManager],
            (TicketStatus.ReturnFromClient, TicketStatus.AwaitingAmApproval) => [Role.AccountManager],
            (TicketStatus.AwaitingClientFeedback, TicketStatus.Completed) => [Role.AccountManager],
            (TicketStatus.Completed, TicketStatus.Closed) => [Role.AccountManager, Role.AgencyAdmin],
            (TicketStatus.OnHold, TicketStatus.AwaitingTraffic) => [Role.TrafficManager],
            (_, TicketStatus.OnHold) => [Role.TrafficManager],
            (_, TicketStatus.Cancelled) => [Role.AgencyAdmin],
            _ => []
        };
    }

    private async Task EnsureClientAccountManagerWatcherAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        var clientResult = await _clientRepository.GetAsync(ticket.ClientId, cancellationToken);
        if (clientResult.IsFailure)
        {
            return;
        }

        var accountManagerId = clientResult.Value.AccountManagerId;
        if (string.IsNullOrWhiteSpace(accountManagerId))
        {
            return;
        }

        if (ticket.Watchers.All(w => w.UserId != accountManagerId))
        {
            ticket.Watchers.Add(new TicketWatcher
            {
                TicketId = ticket.Id,
                UserId = accountManagerId
            });
        }
    }

    private bool IsWorkState(TicketStatus status)
    {
        return status is TicketStatus.InCopy or TicketStatus.InDesign;
    }

    private bool ShouldIncrementRevision(TicketStatus oldStatus, TicketStatus newStatus)
    {
        return newStatus switch
        {
            TicketStatus.InCopy when oldStatus == TicketStatus.ReturnToCopy => true,
            TicketStatus.InDesign when oldStatus == TicketStatus.ReturnToDesign => true,
            TicketStatus.AssignedToCopy when oldStatus == TicketStatus.ReturnFromClient => true,
            TicketStatus.AssignedToDesigner when oldStatus == TicketStatus.ReturnFromClient => true,
            _ => false
        };
    }
}
