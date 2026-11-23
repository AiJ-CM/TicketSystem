using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(100);

        // 预留密码字段，长度根据你的加密算法而定（如 SHA256 通常是 256位）
        builder.Property(u => u.PasswordHash)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(u => u.CreatedAt)
               .HasDefaultValueSql("getutcdate()"); // 设置默认值为当前 UTC 时间

        builder.HasMany(u => u.Roles)
               .WithMany(role => role.Users)
               .UsingEntity<Dictionary<string, object>>(
                   "UserRoles",
                   right => right.HasOne<Role>()
                       .WithMany()
                       .HasForeignKey("RoleId")
                       .HasConstraintName("FK_UserRoles_Roles_RoleId"),
                   left => left.HasOne<User>()
                       .WithMany()
                       .HasForeignKey("UserId")
                       .HasConstraintName("FK_UserRoles_Users_UserId"),
                   join =>
                   {
                       join.ToTable("UserRoles");
                       join.HasKey("UserId", "RoleId");
                   });
    }
}
