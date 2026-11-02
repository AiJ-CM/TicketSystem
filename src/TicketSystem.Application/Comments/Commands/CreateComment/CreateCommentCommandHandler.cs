using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler(ITicketingDbContext context, ICurrentUser currentUser) : IRequestHandler<CreateCommentCommand, long>
{
    public async Task<long> Handle(CreateCommentCommand command, CancellationToken ct)
    {
        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == command.TicketId, ct)
            ?? throw new BusinessException("工单不存在", ErrorCodeEnum.TicketNotFound);

        var comment = new TicketComment
        {
            TicketId = command.TicketId,
            CommenterId = currentUser.UserId,
            Content = command.Content,
            IsInternal = command.IsInternal
        };

        context.TicketComments.Add(comment);

        await context.SaveChangesAsync(ct);

        return comment.Id;
    }
}