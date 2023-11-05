using FileStorage.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorage.Infrastructure.Common.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddFileStorageDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FileStorageDbContext>(options =>
            options.UseNpgsql(configuration["NpgsqlConnectionString"]));

        return services;
    }
}