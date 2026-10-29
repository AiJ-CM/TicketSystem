using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandHandler(ITicketingDbContext context) : IRequestHandler<DeleteTicketCommand>
{
    public async Task Handle(DeleteTicketCommand command, CancellationToken ct)
    {
        // 1. 查询工单
        var ticket = await context.Tickets
            .FirstOrDefaultAsync(t => t.Id == command.TicketId && !t.IsDeleted, ct);

        if (ticket is null)
            throw new KeyNotFoundException("未找到指定的工单");

        // 2. 软删除（BaseEntity 中的字段）
        ticket.IsDeleted = true;
        ticket.DeletedBy = command.OperatorId.ToString();
        ticket.UpdatedBy = command.OperatorId.ToString();

        context.Tickets.Update(ticket);

        // 3. 写入审计日志
        var audit = new TicketAudit
        {
            TicketId = command.TicketId,
            OperatorId = command.OperatorId,
            Action = TicketAuditAction.Deleted,
            BusinessNote = $"软删除了工单（操作人ID: {command.OperatorId}）",
            CreatedAt = DateTime.UtcNow,
        };
        context.TicketAudits.Add(audit);
        await context.SaveChangesAsync(ct);
    }
}