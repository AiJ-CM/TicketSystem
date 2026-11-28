using TicketSystem.Application.Common.StateMachine;

namespace TicketSystem.Application.Tickets.Commands.CloseTicket;

public class CloseTicketCommandHandler(ITicketingDbContext context, TimeProvider timeProvider) : IRequestHandler<CloseTicketCommand>
{
    public async Task Handle(CloseTicketCommand command, CancellationToken ct)
    {
        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == command.TicketId, ct)
            ?? throw new BusinessException("工单不存在", ErrorCodeEnum.TicketNotFound);

        var oldStatus = ticket.Status;
        var (isAllowed, errorCode, message) = TicketStatusTransition.TryTransition(oldStatus, TicketStatus.Closed);
        if (!isAllowed)
            throw new BusinessException(message, errorCode);

        ticket.Status = TicketStatus.Closed;
        ticket.ClosedAt = timeProvider.GetUtcNow().UtcDateTime;

        context.Tickets.Update(ticket);

        await context.SaveChangesAsync(ct);
    }
}