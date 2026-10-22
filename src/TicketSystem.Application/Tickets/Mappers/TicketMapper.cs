using Riok.Mapperly.Abstractions;
using TicketSystem.Application.Tickets.Dtos;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Tickets.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TicketMapper
{
    [MapProperty(nameof(Ticket.Creator.DisplayName), nameof(TicketDto.CreatorName))]
    [MapProperty(nameof(Ticket.Assignee.DisplayName), nameof(TicketDto.AssigneeName))]
    public static partial TicketDto ToDtoWithUserInfo(Ticket ticket);
}