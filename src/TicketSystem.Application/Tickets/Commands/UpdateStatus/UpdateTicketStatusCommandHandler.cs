using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == command.TicketId, cancellationToken)
            ?? throw new BusinessException("工单不存在", ErrorCodeEnum.TicketNotFound);

        var oldStatus = ticket.Status;
        var (isAllowed, errorCode, message) = TicketStatusTransition.TryTransition(oldStatus, command.NewStatus);
        if (!isAllowed)
            throw new BusinessException(message, errorCode);

        ticket.Status = command.NewStatus;

        await context.SaveChangesAsync(cancellationToken);
    }
}