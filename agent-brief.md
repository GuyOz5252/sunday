# Backend Implementation Brief

## Sunday — Coding Agent Instructions

> C# / .NET 10 · Clean Architecture · EF Core 10

---

## Solution Structure

```
Sunday.sln
├── Sunday.Core/           ← Domain layer. No external dependencies.
├── Sunday.Application/    ← Use cases, CQRS handlers, DTOs, validators.
├── Sunday.Data/           ← EF Core, repository implementations, migrations.
└── Sunday.Api/            ← Controllers, middleware, DI root.
```

Dependency rule:

```
Api        → Application → Core
Data       → Core
```

---

## Domain Layer (Sunday.Core)

### Aggregate Roots & Owned Entities

#### Ticket

```csharp
public class Ticket
{
    public string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? CurrentAssigneeId { get; init; }     // denormalized from active assignment
    public WorkStatus WorkStatus { get; init; }
    public ApprovalStatus ApprovalStatus { get; init; }
    public int CurrentStageIndex { get; init; }
    public List<TicketAssignment> Assignments { get; init; } = [];
    public List<ReviewStage> ReviewStages { get; init; } = [];
    public List<WorkSession> WorkSessions { get; init; } = [];
    public List<TicketComment> Comments { get; init; } = [];
}

// Owned by Ticket — no DbSet
public class TicketAssignment
{
    public string Id { get; init; }
    public string AssigneeId { get; init; }
    public string AssignedById { get; init; }
    public DateTimeOffset AssignedAt { get; init; }
    public DateTimeOffset? EndedAt { get; init; }
    public AssignmentEndReason? EndReason { get; init; }
    public string? Notes { get; init; }
}

// Owned by Ticket — no DbSet
public class ReviewStage
{
    public string Id { get; init; }
    public int Index { get; init; }
    public string Name { get; init; }
    public List<string> ApproverIds { get; init; } = [];  // stored as JSON column
    public int RequiredApprovals { get; init; }
    public StageDecision? Decision { get; init; }
    public string? DecidedById { get; init; }
    public DateTimeOffset? DecidedAt { get; init; }
    public string? Notes { get; init; }
}

// Owned by Ticket — no DbSet
public class WorkSession
{
    public string Id { get; init; }
    public string WorkerId { get; init; }
    public string AssignmentId { get; init; }           // FK to the TicketAssignment active at start
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset? EndedAt { get; init; }
    public long? DurationSeconds { get; init; }         // sealed on close, never edited
    public string? Notes { get; init; }
}

// Owned by Ticket — no DbSet
public class TicketComment
{
    public string Id { get; init; }
    public string AuthorId { get; init; }
    public string Content { get; init; }
    public bool IsSystemMessage { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
```

#### User

```csharp
public class User
{
    public string Id { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public string PasswordHash { get; init; }
    public GlobalRole GlobalRole { get; init; }
}
```

#### Workspace

```csharp
public class Workspace
{
    public string Id { get; init; }
    public required string Name { get; init; }
    public required string OwnerId { get; init; }
}
```

---

### Enums

```csharp
public enum BoardRole           { Viewer, Worker, Approver, Admin }
public enum GlobalRole          { Admin, Member }
public enum WorkStatus          { Pending, InProgress, Break, Done }
public enum ApprovalStatus      { AwaitingAssignment, InReview, Rejected, RedoRequested, Completed }
public enum StageDecision       { Approved, Rejected, RedoRequested }
public enum AssignmentEndReason { Reassigned, TaskCompleted, TaskRejected }
```

---

### Domain Events

Raised by domain methods. Published by handlers after `SaveChangesAsync`.

```csharp
public interface IDomainEvent { }

BoardCreatedEvent               { BoardId, WorkspaceId, OwnerId }
TicketCreatedEvent              { TicketId, BoardId, CreatedById }
TicketAssignedEvent             { TicketId, AssignmentId, AssigneeId, AssignedById }
TicketReassignedEvent           { TicketId, NewAssignmentId, NewAssigneeId, PreviousAssigneeId, ClosedSessionId? }
WorkStatusChangedEvent          { TicketId, OldStatus, NewStatus, ChangedById }
// ↑ Handler for this event opens WorkSession when NewStatus == InProgress
// ↑ Handler for this event closes WorkSession when NewStatus == Break | Done
TicketSubmittedForReviewEvent   { TicketId, StageIndex }
StageApprovedEvent              { TicketId, StageIndex, ApproverId }
StageRejectedEvent              { TicketId, StageIndex, ApproverId, Reason }
RedoRequestedEvent              { TicketId, StageIndex, ApproverId, Instructions }
TicketCompletedEvent            { TicketId }
WorkSessionEndedEvent           { TicketId, SessionId, WorkerId, DurationSeconds }
```

---

### Repository Interfaces

Defined in Core. Implemented in Data.

```csharp
public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(string id, CancellationToken ct);
    Task AddAsync(Board board, CancellationToken ct);
    Task UpdateAsync(Board board, CancellationToken ct);
}

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(string id, CancellationToken ct);
    Task<IReadOnlyList<Ticket>> GetByBoardAsync(string boardId, CancellationToken ct);
    Task<IReadOnlyList<Ticket>> GetByAssigneeAsync(string assigneeId, CancellationToken ct);
    Task AddAsync(Ticket ticket, CancellationToken ct);
    Task UpdateAsync(Ticket ticket, CancellationToken ct);
}

public interface IWorkspaceRepository
{
    Task<Workspace?> GetByIdAsync(string id, CancellationToken ct);
}

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken ct);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}
```

---

### Base Types

```csharp
public abstract class AggregateRoot
{
    public string Id { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
```

---

## Application Layer (Sunday.Application)

### MediatR Interfaces

```csharp
public interface ICommand<TResult> : IRequest<TResult> { }
public interface IQuery<TResult> : IRequest<TResult> { }
```

### Pipeline Behaviors (registered in order)

```
1. LoggingBehavior        ← logs command/query name and duration
2. ValidationBehavior     ← runs FluentValidation, throws on failure
3. AuthorizationBehavior  ← enforces permissions per command
4. TransactionBehavior    ← wraps commands in a DB transaction
```

Domain events are published by each handler manually after `SaveChangesAsync`. Use an outbox pattern later for production reliability.

---

### Command Handlers

#### Board Handlers

**CreateBoardHandler**

- Command: `{ WorkspaceId, Name, RequesterId }`
- Load workspace, assert requester is workspace owner
- Create `Board` with owner auto-added as `BoardRole.Admin` member
- Save, publish `BoardCreatedEvent`
- Return: `string` (board Id)

**AddBoardMemberHandler**

- Command: `{ BoardId, UserId, Role, RequesterId }`
- Load board, assert requester has `BoardRole.Admin`
- Assert user exists
- Assert user not already a member
- Add `BoardMember { UserId, Role }` to board
- Save

**RemoveBoardMemberHandler**

- Command: `{ BoardId, UserId, RequesterId }`
- Load board, assert requester has `BoardRole.Admin`
- Assert target user is not the board owner
- Remove member from board
- Save

---

#### Ticket Handlers

**CreateTicketHandler**

- Command: `{ BoardId, Title, Description, ReviewStages: List<ReviewStageInput>, RequesterId }`
- Load board, assert requester is a board member with `Worker` or `Admin` role
- Assert each `ApproverIds` in stages are board members with `Approver` or `Admin` role
- Create `Ticket` with `ApprovalStatus = AwaitingAssignment`, `WorkStatus = Pending`
- Save, publish `TicketCreatedEvent`
- Return: `string` (ticket Id)

**AssignTicketHandler**

- Command: `{ TicketId, AssigneeId, Notes?, RequesterId }`
- Load ticket, assert `ApprovalStatus == AwaitingAssignment`
- Assert requester has `BoardRole.Admin` on the ticket's board
- Assert assignee has `BoardRole.Worker` on the ticket's board
- Create `TicketAssignment { AssigneeId, AssignedById = RequesterId, AssignedAt = UtcNow }`
- Set `ticket.CurrentAssigneeId = AssigneeId`
- Set `ticket.ApprovalStatus = InReview` (ready for work)
- Save, publish `TicketAssignedEvent`

**ReassignTicketHandler**

- Command: `{ TicketId, NewAssigneeId, Notes?, RequesterId }`
- Load ticket, assert requester has `BoardRole.Admin`
- Assert `NewAssigneeId != CurrentAssigneeId`
- Assert new assignee has `BoardRole.Worker`
- If open `WorkSession` exists: force-close it (set `EndedAt`, compute `DurationSeconds`)
- Close active `TicketAssignment` (`EndedAt = UtcNow`, `EndReason = Reassigned`)
- Create new `TicketAssignment`
- Set `ticket.CurrentAssigneeId = NewAssigneeId`
- Reset `WorkStatus = Pending`
- Save, publish `TicketReassignedEvent`

**ChangeWorkStatusHandler**

- Command: `{ TicketId, NewStatus, RequesterId }`
- Load ticket, assert requester == `CurrentAssigneeId`
- Assert transition is valid (see state machine below)
- Update `ticket.WorkStatus`
- Save, publish `WorkStatusChangedEvent`
- **WorkSession is NOT managed here** — it is managed by a domain event handler:
    - `WorkStatusChangedEvent` where `NewStatus == InProgress` → open `WorkSession`
    - `WorkStatusChangedEvent` where `NewStatus == Break | Done` → close open `WorkSession`

**SubmitTicketForReviewHandler**

- Command: `{ TicketId, RequesterId }`
- Load ticket, assert requester == `CurrentAssigneeId`
- Assert `WorkStatus == Done`
- Assert `ApprovalStatus` is not `Completed` or `Rejected`
- Set `ApprovalStatus = InReview`
- Save, publish `TicketSubmittedForReviewEvent`

---

#### Review & Approval Handlers

**ApproveStageHandler**

- Command: `{ TicketId, StageIndex, ApproverId, Notes? }`
- Load ticket, assert `ApprovalStatus == InReview`
- Assert `StageIndex == CurrentStageIndex`
- Assert `ApproverId` is in `ReviewStage.ApproverIds` for that stage
- Record decision on stage (`Decision = Approved`, `DecidedById`, `DecidedAt`)
- If more stages remain: `CurrentStageIndex++`
- If no more stages: `ApprovalStatus = Completed`, publish `TicketCompletedEvent`
- Save, publish `StageApprovedEvent`

**RejectTicketHandler**

- Command: `{ TicketId, StageIndex, ApproverId, Reason }`
- Load ticket, assert `ApprovalStatus == InReview`
- Assert `StageIndex == CurrentStageIndex`
- Assert `ApproverId` is in `ReviewStage.ApproverIds` for that stage
- Set `ApprovalStatus = Rejected`
- Record decision on stage
- Save, publish `StageRejectedEvent`

**RequestRedoHandler**

- Command: `{ TicketId, StageIndex, ApproverId, Instructions }`
- Load ticket, assert `ApprovalStatus == InReview`
- Assert `StageIndex == CurrentStageIndex`
- Assert `ApproverId` is in `ReviewStage.ApproverIds` for that stage
- Set `ApprovalStatus = RedoRequested`
- Set `WorkStatus = Pending`
- Record decision on stage with instructions
- Save, publish `RedoRequestedEvent`

---

#### Work Session Handlers

**EndWorkSessionHandler**

- Command: `{ TicketId, SessionId, Notes?, RequesterId }`
- Load ticket, find session by `SessionId`
- Assert requester == `session.WorkerId`
- Assert session is still open (`EndedAt == null`)
- Set `EndedAt = UtcNow`, compute `DurationSeconds`
- Session is now sealed — never edited after this point
- Save, publish `WorkSessionEndedEvent`

---

### WorkStatus Transition Rules

```
Pending     → InProgress   (worker starts)
InProgress  → Break        (worker pauses)
InProgress  → Done         (worker finishes)
Break       → InProgress   (worker resumes)
Done        → InProgress   (worker reopens — e.g. after redo requested)
```

Any other transition is invalid and should throw `DomainException`.

---

### ApprovalStatus Transition Rules

```
AwaitingAssignment → InReview       (ticket assigned)
InReview           → Completed      (all stages approved)
InReview           → Rejected       (hard reject)
InReview           → RedoRequested  (sent back for fixes)
RedoRequested      → InReview       (worker resubmits after fixing)
```

---

## Data Layer (Sunday.Data)

### DbContext

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
}
```

### EF Configuration Notes

**Board**

- `BoardMembers` configured via `OwnsMany` — shadow FK `BoardId`, composite PK `(BoardId, UserId)`
- `Id` is `ValueGeneratedOnAdd()`

**Ticket**

- `TicketAssignments`, `ReviewStages`, `WorkSessions`, `Comments` all configured via `OwnsMany`
- `ReviewStage.ApproverIds` stored as JSON column
- All enums stored as `string` via `.HasConversion<string>()`
- `Id` is `ValueGeneratedOnAdd()`

**Key Indices**

```
Tickets(BoardId)
Tickets(CurrentAssigneeId)
TicketAssignments(TicketId) WHERE EndedAt IS NULL   ← unique partial, enforces one active assignment
WorkSessions(TicketId, WorkerId) WHERE EndedAt IS NULL ← unique partial, enforces one open session per worker
ReviewStages(TicketId, Index)
```

---

## API Layer (Sunday.API)

### Middleware Stack (in order)

```
1. ExceptionHandlingMiddleware   ← maps exceptions to HTTP status codes
2. JwtAuthenticationMiddleware   ← validates Bearer token
3. RequestContextMiddleware      ← extracts UserId → ICurrentUser
```

### Exception Mapping

```
DomainException        → 400 Bad Request
UnauthorizedException  → 403 Forbidden
NotFoundException      → 404 Not Found
ValidationException    → 422 Unprocessable Entity
```

### ICurrentUser

```csharp
public interface ICurrentUser
{
    string UserId { get; }
    GlobalRole GlobalRole { get; }
}
```

Registered as `Scoped`. Populated by `RequestContextMiddleware` from JWT claims.
