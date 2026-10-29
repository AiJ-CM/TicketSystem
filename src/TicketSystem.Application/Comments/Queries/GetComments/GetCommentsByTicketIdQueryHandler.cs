using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Comments.Dtos;
using TicketSystem.Application.Comments.Mappers;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Comments.Queries.GetComments;

public class GetCommentsByTicketIdQueryHandler(ITicketingDbContext context) : IRequestHandler<GetCommentsByTicketIdQuery, IEnumerable<CommentDto>>
{
    public async Task<IEnumerable<CommentDto>> Handle(GetCommentsByTicketIdQuery query, CancellationToken ct)
    {
        var comments = await context.TicketComments
            .AsNoTracking()
            .Include(c => c.Commenter)
            .Where(c => c.TicketId == query.TicketId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);


        return comments.Select(c => CommentMapper.ToDto(c));
    }
}