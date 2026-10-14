using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TicketSystem.Infrastructure.Data;

public class TicketingDbContextFactory : IDesignTimeDbContextFactory<TicketingDbContext>
{
    public TicketingDbContext CreateDbContext(string[] args)
    {
        // 数据库链接字符串
        var connectionString = "";

        var optionsBuilder = new DbContextOptionsBuilder<TicketingDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new TicketingDbContext(optionsBuilder.Options);
    }
}