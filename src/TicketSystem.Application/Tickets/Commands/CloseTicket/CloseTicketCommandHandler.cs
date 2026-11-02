using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.StateMachine;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.CloseTicket;

public class CloseTicketCommandHandler(ITicketingDbContext context) : IRequestHandler<CloseTicketCommand>
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
        ticket.ClosedAt = DateTime.UtcNow;

        context.Tickets.Update(ticket);

        await context.SaveChangesAsync(ct);
    }
}