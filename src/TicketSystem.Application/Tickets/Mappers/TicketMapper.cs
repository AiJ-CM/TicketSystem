using Riok.Mapperly.Abstractions;
using TicketSystem.Application.Tickets.Dtos;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Tickets.Mappers;

[Mapper]
public static partial class TicketMapper
{
    // 告诉 Mapperly：
    // 从 ticket.Creator.DisplayName 赋值给 dto.CreatorName
    // 从 ticket.Assignee.DisplayName 赋值给 dto.AssigneeName
    [MapProperty(nameof(Ticket.Creator) + "." + "DisplayName", nameof(TicketDto.CreatorName))]
    [MapProperty(nameof(Ticket.Assignee) + "." + "DisplayName", nameof(TicketDto.AssigneeName))]
    [MapperIgnoreSource(nameof(Ticket.RowVersion))]
    public static partial TicketDto ToDtoWithUserInfo(Ticket ticket);
}