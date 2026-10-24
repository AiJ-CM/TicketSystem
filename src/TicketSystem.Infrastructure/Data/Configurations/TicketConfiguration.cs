using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Infrastructure.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.CreatorId)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.TicketType)
            .HasConversion<string>()
            .HasMaxLength(50);


        // 关键：把 RowVersion 配置成 SQL Server 的 rowversion 并发令牌
        builder.Property(t => t.RowVersion)
            .IsRowVersion();

        // 👇 配置外键约束
        builder.HasOne(t => t.Creator)
               .WithMany(u => u.CreatedTickets)
               .HasForeignKey(t => t.CreatorId)
               .OnDelete(DeleteBehavior.Restrict);

        // 2. 负责人 (Assignee) - 可以为空 (IsRequired(false))
        builder.HasOne(t => t.Assignee)
               .WithMany(u => u.AssignedTickets)
               .HasForeignKey(t => t.AssigneeId)
               .IsRequired(false);
    }
}