using TicketSystem.Application.Tickets.Dtos;
using TicketSystem.Application.Tickets.Mappers;

namespace TicketSystem.Application.Tickets.Queries.GetTicket;

public class GetTicketQueryHandler(ITicketingDbContext context) : IRequestHandler<GetTicketQuery, TicketDto>
{
    public async Task<TicketDto> Handle(GetTicketQuery request, CancellationToken cancellationToken)
    {
        var ticket = await context.Tickets
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        return ticket is null
            ? throw new BusinessException($"工单 {request.Id} 不存在", ErrorCodeEnum.TicketNotFound)
            : TicketMapper.ToDtoWithUserInfo(ticket);
    }
}