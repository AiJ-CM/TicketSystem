using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.StateMachine;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.SubmitResolution;

public class SubmitResolutionCommandHandler(ITicketingDbContext context) : IRequestHandler<SubmitResolutionCommand>
{
    public async Task Handle(SubmitResolutionCommand command, CancellationToken ct)
    {
        var ticket = await context.Tickets
            .FirstOrDefaultAsync(t => t.Id == command.TicketId && !t.IsDeleted, ct);

        if (ticket is null)
            throw new KeyNotFoundException("未找到指定的工单");

        var oldStatus = ticket.Status;
        var newStatus = TicketStatus.Resolved;
        var (isAllowed, errorCode, message) = TicketStatusTransition.TryTransition(oldStatus, newStatus);
        if (!isAllowed)
            throw new BusinessException(message, errorCode);

        if (!isAllowed)
        {
            throw new BusinessException(message, errorCode);
        }

        ticket.Status = newStatus;
        ticket.ClosedAt = DateTime.UtcNow;
        ticket.UpdatedBy = command.OperatorId.ToString();

        context.Tickets.Update(ticket);

        var audit = new TicketAudit
        {
            TicketId = command.TicketId,
            OperatorId = command.OperatorId,
            Action = TicketAuditAction.Resolved,
            OldValue = oldStatus.ToString(),
            NewValue = newStatus.ToString(),
            BusinessNote = command.ResolutionNote ?? $"提交了解决（操作人ID: {command.OperatorId}）",
            CreatedAt = DateTime.UtcNow,
        };
        context.TicketAudits.Add(audit);
        await context.SaveChangesAsync(ct);
    }
}