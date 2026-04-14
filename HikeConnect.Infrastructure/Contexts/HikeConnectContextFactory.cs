using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HikeConnect.Infrastructure.Contexts;

public class HikeConnectContextFactory : IDesignTimeDbContextFactory<HikeConnectContext>
{
    public HikeConnectContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HikeConnectContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=hikeconnect;Username=ulquaza;Password=blondesarethebest");

        return new HikeConnectContext(optionsBuilder.Options);
    }
}