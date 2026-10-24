using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Common.Interfaces;

public interface ITicketingDbContext
{
    DbSet<User> Users { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<TicketComment> TicketComments { get; }
    DbSet<TicketAudit> TicketAudits { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}