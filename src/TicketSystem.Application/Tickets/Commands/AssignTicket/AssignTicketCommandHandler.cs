using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.AssignTicket;

public class AssignTicketCommandHandler(ITicketingDbContext context) : IRequestHandler<AssignTicketCommand>
{
    public async Task Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
            ?? throw new BusinessException("工单不存在", ErrorCodeEnum.TicketNotFound);

        var oldAssigneeId = ticket.AssigneeId;
        ticket.AssigneeId = request.AssigneeId;
        var businessNote = oldAssigneeId.HasValue
            ? $"将工单从 '{oldAssigneeId}' 重新指派给了 '{request.AssigneeId}'"
            : $"将工单指派给了 '{request.AssigneeId}'";
        var auditLog = new TicketAudit
        {
            TicketId = ticket.Id,
            OperatorId = request.CurrentUserId,
            Action = TicketAuditAction.Assigned,
            OldValue = oldAssigneeId?.ToString(),
            NewValue = request.AssigneeId.ToString(),
            BusinessNote = businessNote,
            CreatedAt = DateTime.UtcNow
        };

        context.TicketAudits.Add(auditLog);
        await context.SaveChangesAsync(cancellationToken);
    }
}