using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Tickets.Commands.AssignTicket;

public class AssignTicketCommandHandler(ITicketingDbContext context) : IRequestHandler<AssignTicketCommand>
{
    public async Task Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await context.Tickets
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken)
            ?? throw new BusinessException("工单不存在", ErrorCodeEnum.TicketNotFound);

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.AssigneeId, cancellationToken)
            ?? throw new BusinessException("用户不存在", ErrorCodeEnum.UserNotFound);

        if (ticket.AssigneeId == request.AssigneeId)
        {
            throw new BusinessException("工单已指派给该用户", ErrorCodeEnum.TicketAlreadyAssigned);
        }

        ticket.AssigneeId = request.AssigneeId;

        await context.SaveChangesAsync(cancellationToken);
    }
}