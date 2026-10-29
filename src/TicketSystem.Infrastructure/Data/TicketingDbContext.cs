using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Entities.Common;

namespace TicketSystem.Infrastructure.Data;

public class TicketingDbContext(DbContextOptions<TicketingDbContext> options) : DbContext(options), ITicketingDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();
    public DbSet<TicketAudit> TicketAudits => Set<TicketAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketingDbContext).Assembly);
        modelBuilder.Entity<BaseEntity>().HasQueryFilter(u => !u.IsDeleted);
    }
}