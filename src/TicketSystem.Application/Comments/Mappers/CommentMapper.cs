using Riok.Mapperly.Abstractions;
using TicketSystem.Application.Comments.Dtos;
using TicketSystem.Application.Mappers;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Comments.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class CommentMapper
{
    [MapProperty(
        nameof(TicketComment.Commenter), 
        nameof(CommentDto.CommenterName),
        Use = nameof(@UserMapper.GetName))]
    public static partial CommentDto ToDto(TicketComment comment);
}