using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandHandler(ITicketingDbContext context) : IRequestHandler<DeleteTicketCommand>
{
    public async Task Handle(DeleteTicketCommand command, CancellationToken ct)
    {
        var ticket = await context.Tickets
            .FirstOrDefaultAsync(t => t.Id == command.TicketId && !t.IsDeleted, ct)
            ?? throw new KeyNotFoundException("未找到指定的工单");

        context.Tickets.Remove(ticket);

        // 3. 写入审计日志
        //var audit = new TicketAudit
        //{
        //    TicketId = command.TicketId,
        //    OperatorId = command.OperatorId,
        //    Action = TicketAuditAction.Deleted,
        //    BusinessNote = $"软删除了工单（操作人ID: {command.OperatorId}）",
        //    CreatedAt = DateTime.UtcNow,
        //};

        await context.SaveChangesAsync(ct);
    }
}