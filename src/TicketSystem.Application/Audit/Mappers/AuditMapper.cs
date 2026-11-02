using Riok.Mapperly.Abstractions;
using TicketSystem.Application.Audit.Dtos;
using TicketSystem.Application.Mappers;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Audit.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AuditMapper
{
    [MapProperty(nameof(TicketAudit.Ticket.Title), nameof(AuditDto.TicketTitle))]
    [MapProperty(nameof(TicketAudit.Operator), nameof(AuditDto.OperatorName), Use = nameof(@UserMapper.GetName))]
    public static partial AuditDto ToAuditDto(TicketAudit auditDto);
}