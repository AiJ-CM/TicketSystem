using MediatR;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.StateMachine;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.UpdateStatus;

public class UpdateTicketStatusHandler(ITicketingDbContext context) : IRequestHandler<UpdateTicketStatusCommand>
{
    public async Task Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await context.Tickets.FindAsync([request.TicketId], cancellationToken)
            ?? throw new BusinessException("工单不存在", ErrorCodeEnum.TicketNotFound);

        var oldStatus = ticket.Status;
        var (isAllowed, errorCode, message) = TicketStatusTransition.TryTransition(oldStatus, request.NewStatus);
        if (!isAllowed)
            throw new BusinessException(message, errorCode);

        if (!isAllowed)
        {
            throw new BusinessException(message, errorCode);
        }

        ticket.Status = request.NewStatus;

        context.TicketAudits.Add(new TicketAudit
        {
            TicketId = ticket.Id,
            OperatorId = request.CurrentUserId,
            Action = TicketAuditAction.StatusChanged,
            OldValue = oldStatus.ToString(),
            NewValue = request.NewStatus.ToString(),
            BusinessNote = $"状态变更: {oldStatus} -> {request.NewStatus}",
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}