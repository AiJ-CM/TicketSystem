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
        // 1. 查询工单
        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == command.TicketId, ct)
            ?? throw new BusinessException("工单不存在", ErrorCodeEnum.TicketNotFound);


        // 2. 状态流转校验
        var oldStatus = ticket.Status;
        var (isAllowed, errorCode, message) = TicketStatusTransition.TryTransition(oldStatus, TicketStatus.Closed);
        if (!isAllowed)
            throw new BusinessException(message, errorCode);

        if (!isAllowed)
        {
            throw new BusinessException(message, errorCode);
        }

        // 3. 记录旧值
        var oldValue = ticket.Status.ToString();

        // 4. 更新状态
        ticket.Status = TicketStatus.Closed;
        ticket.ClosedAt = DateTime.UtcNow;
        ticket.UpdatedBy = command.OperatorId.ToString();

        context.Tickets.Update(ticket);

        // 5. 写入审计日志
        var audit = new TicketAudit
        {
            TicketId = command.TicketId,
            OperatorId = command.OperatorId,
            Action = TicketAuditAction.Closed,
            OldValue = oldValue,
            NewValue = TicketStatus.Closed.ToString(),
            BusinessNote = $"确认关闭了工单（操作人ID: {command.OperatorId}）",
            CreatedAt = DateTime.UtcNow,
        };
        context.TicketAudits.Add(audit);
        await context.SaveChangesAsync(ct);
    }
}