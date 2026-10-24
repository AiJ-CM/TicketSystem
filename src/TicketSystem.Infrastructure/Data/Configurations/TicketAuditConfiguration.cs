using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Infrastructure.Data.Configurations;

public class TicketAuditConfiguration : IEntityTypeConfiguration<TicketAudit>
{
    public void Configure(EntityTypeBuilder<TicketAudit> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Action)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.OldValue).HasMaxLength(200);
        builder.Property(a => a.NewValue).HasMaxLength(200);

        builder.Property(a => a.BusinessNote)
            .HasMaxLength(500);

        builder.HasOne(a => a.Ticket)
            .WithMany(t => t.Audits)
            .HasForeignKey(a => a.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Operator)
            .WithMany(u => u.Audits)
            .HasForeignKey(a => a.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}