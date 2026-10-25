using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Tickets.Dtos;
using TicketSystem.Application.Tickets.Mappers;

namespace TicketSystem.Application.Tickets.Queries.GetTicketList;

public class GetTicketListQueryHandler(
    ITicketingDbContext context) : IRequestHandler<GetTicketListQuery, PagedList<TicketDto>>
{
    public async Task<PagedList<TicketDto>> Handle(GetTicketListQuery request, CancellationToken cancellationToken)
    {
        var query = context.Tickets
            .Include(t => t.Creator)
            .Include(t => t.Assignee)
            .AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }
        if (request.AssignedTo.HasValue)
        {
            query = query.Where(t => t.AssigneeId == request.AssignedTo.Value);
        }
        if (request.CreatedBy.HasValue)
        {
            query = query.Where(t => t.CreatorId == request.CreatedBy.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var tickets = await query
            .OrderByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = tickets.Select(TicketMapper.ToDtoWithUserInfo).ToList();

        return new PagedList<TicketDto>(items, totalItems, request.PageIndex, request.PageSize);
    }
}