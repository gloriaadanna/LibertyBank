using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure;

public static class ServiceExtension
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Database:ConnectionString"];
        var databasePassword = configuration["Database:DbPassword"];
        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Password = databasePassword
        };
        services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(builder.ConnectionString));
        return services;
    }
}