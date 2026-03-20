using DotResults;
using Microsoft.EntityFrameworkCore;
using Sunday.Core.Abstracts;
using Sunday.Core.DomainEvents;
using Sunday.Core.Models;

namespace Sunday.Data.Repositories;

public class EfCoreTicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;

    public EfCoreTicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public System.Threading.Tasks.Task<string> CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        _context.Tickets.Add(ticket);
        return System.Threading.Tasks.Task.FromResult(ticket.Id);
    }

    public async Task<Result<Ticket>> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Assignments)
            .Include(t => t.Watchers)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return ticket is null
            ? Result.Failure<Ticket>(new Error("Ticket.NotFound", $"Ticket with id {id} not found", "NotFound"))
            : Result.Success(ticket);
    }

    public async Task<Result<List<Ticket>>> GetByAgencyAsync(string agencyId, int skip, int take, CancellationToken cancellationToken = default)
    {
        var tickets = await _context.Tickets
            .Where(t => t.AgencyId == agencyId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return Result.Success(tickets);
    }

    public Task<string> CreateWorkSessionAsync(WorkSession session, CancellationToken cancellationToken = default)
    {
        _context.WorkSessions.Add(session);
        return Task.FromResult(session.Id);
    }

    public async Task<Result<WorkSession>> GetActiveWorkSessionAsync(string userId, CancellationToken cancellationToken = default)
    {
        var session = await _context.WorkSessions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.EndTime == null, cancellationToken);

        return session is null
            ? Result.Failure<WorkSession>(new Error("WorkSession.NotFound", $"Active work session for user {userId} not found", "NotFound"))
            : Result.Success(session);
    }

    public async Task<Result<List<WorkSession>>> GetWorkSessionsByTicketAsync(string ticketId, CancellationToken cancellationToken = default)
    {
        var sessions = await _context.WorkSessions
            .Where(s => s.TicketId == ticketId)
            .ToListAsync(cancellationToken);

        return Result.Success(sessions);
    }
}
