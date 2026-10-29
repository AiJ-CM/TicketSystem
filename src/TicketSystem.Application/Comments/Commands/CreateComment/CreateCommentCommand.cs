using MediatR;

namespace TicketSystem.Application.Comments.Commands.CreateComment;

public record CreateCommentCommand(
    long TicketId,
    string Content ,
    bool IsInternal 
) : IRequest<long>;