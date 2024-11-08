using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Common.EventBus;

public static class PostgresExtensions
{
    public static DbContextOptionsBuilder UseIntegrationContext(this DbContextOptionsBuilder options, IConfiguration configuration, string appName)
    {
        options.UseNpgsql(configuration.GetConnectionString("Default"), b => b.MigrationsAssembly(appName));
        return options;
    }
}