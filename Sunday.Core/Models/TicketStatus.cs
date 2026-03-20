namespace Sunday.Core.Models;

public enum TicketStatus
{
    Draft,
    New,
    AwaitingTraffic,
    AwaitingBriefCompletion,
    AssignedToCopy,
    InCopy,
    ReadyForDesign,
    AssignedToDesigner,
    InDesign,
    AwaitingCreativeApproval,
    AwaitingBriefClarification,
    ReturnToDesign,
    ReturnToCopy,
    AwaitingAmApproval,
    SentToClient,
    AwaitingClientFeedback,
    ReturnFromClient,
    Completed,
    Closed,
    Cancelled,
    OnHold
}
