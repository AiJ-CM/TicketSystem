using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.StateMachine;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.UpdateStatus;

public class UpdateTicketStatusCommandHandler(ITicketingDbContext context) : IRequestHandler<UpdateTicketStatusCommand>
{
    public async Task Handle(UpdateTicketStatusCommand command, CancellationToken cancellationToken)
    {
        var ticket = await context.Tickets.FindAsync([command.TicketId], cancellationToken)
            ?? throw new BusinessException("工单不存在", ErrorCodeEnum.TicketNotFound);

        var oldStatus = ticket.Status;
        var (isAllowed, errorCode, message) = TicketStatusTransition.TryTransition(oldStatus, command.NewStatus);
        if (!isAllowed)
            throw new BusinessException(message, errorCode);

        if (!isAllowed)
        {
            throw new BusinessException(message, errorCode);
        }

        ticket.Status = command.NewStatus;

        context.TicketAudits.Add(new TicketAudit
        {
            TicketId = ticket.Id,
            OperatorId = command.CurrentUserId,
            Action = TicketAuditAction.StatusChanged,
            OldValue = oldStatus.ToString(),
            NewValue = command.NewStatus.ToString(),
            BusinessNote = $"状态变更: {oldStatus} -> {command.NewStatus}",
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}