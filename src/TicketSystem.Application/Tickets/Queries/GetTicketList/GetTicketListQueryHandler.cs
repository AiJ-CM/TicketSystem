using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Application.Common.Models;
using TicketSystem.Application.Tickets.Dtos;
using TicketSystem.Application.Tickets.Mappers;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Tickets.Queries.GetTicketList;

internal class GetTicketListQueryHandler(
    ITicketingDbContext context) : IRequestHandler<GetTicketListQuery, PagedList<TicketDto>>
{
    public async Task<PagedList<TicketDto>> Handle(GetTicketListQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Ticket> query = context.Tickets
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt); // 默认按创建时间倒序

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
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = tickets.Select(TicketMapper.ToDtoWithUserInfo).ToList();

        return new PagedList<TicketDto>(items, totalItems, request.PageIndex, request.PageSize);
    }
}