using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicketSystem.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.PermissionCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(permission => permission.PermissionCode)
            .IsUnique();

        builder.Property(permission => permission.ModuleName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(permission => permission.Description)
            .IsRequired()
            .HasMaxLength(500);
    }
}
