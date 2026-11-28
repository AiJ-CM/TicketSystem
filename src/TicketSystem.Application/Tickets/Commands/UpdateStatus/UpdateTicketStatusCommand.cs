namespace TicketSystem.Application.Tickets.Commands.UpdateStatus;

public record UpdateTicketStatusCommand(
    long TicketId,
    TicketStatus NewStatus
) : IRequest;