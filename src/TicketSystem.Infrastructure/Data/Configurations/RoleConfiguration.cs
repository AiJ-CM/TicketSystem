using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.RoleName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(role => role.RoleName)
            .IsUnique();

        builder.Property(role => role.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasMany(role => role.Permissions)
            .WithMany(permission => permission.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "RolePermissions",
                right => right.HasOne<Permission>()
                    .WithMany()
                    .HasForeignKey("PermissionId")
                    .HasConstraintName("FK_RolePermissions_Permissions_PermissionId"),
                left => left.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .HasConstraintName("FK_RolePermissions_Roles_RoleId"),
                join =>
                {
                    join.ToTable("RolePermissions");
                    join.HasKey("RoleId", "PermissionId");
                });
    }
}
