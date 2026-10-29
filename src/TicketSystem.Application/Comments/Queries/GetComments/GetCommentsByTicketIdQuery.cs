using MediatR;
using TicketSystem.Application.Comments.Dtos;

namespace TicketSystem.Application.Comments.Queries.GetComments;

public record GetCommentsByTicketIdQuery(long TicketId) : IRequest<IEnumerable<CommentDto>>;