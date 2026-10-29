using MediatR;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler(
    ITicketingDbContext context, ICurrentUser currentUser) : IRequestHandler<CreateTicketCommand, long>
{
    public async Task<long> Handle(
        CreateTicketCommand request, CancellationToken ct)
    {
        var currentId = currentUser.UserId;
        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            TicketType = request.TicketType,
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CreatorId = currentId
        };

        context.Tickets.Add(ticket);

        //await auditLogService.LogAsync(
        //    ticket.Id,
        //    TicketAuditAction.AddedComment,
        //    $"创建了工单（操作人ID: {currentId}）"
        //);

        await context.SaveChangesAsync(ct);

        return ticket.Id;
    }
}