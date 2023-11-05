using FileStorage.Domain.Common;
using FileStorage.Domain.Entities;
using FileStorage.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using File = FileStorage.Domain.Entities.File;

namespace FileStorage.Infrastructure.Data;

/// <summary>
/// Контекст базы данных.
/// </summary>
public class FileStorageDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public FileStorageDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Файлы
    /// </summary>
    public DbSet<File> Files { get; set; } = null!;

    /// <summary>
    /// Группы файлов
    /// </summary>
    public DbSet<FileGroup> FileGroups { get; set; } = null!;

    /// <summary>
    /// Пользователи
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Временные ссылки
    /// </summary>
    public DbSet<TemporaryLink> TemporaryLinks { get; set; } = null!;

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // для генерации гуидов
        modelBuilder.HasPostgresExtension("uuid-ossp");
        
        modelBuilder.ApplyConfiguration(new FileConfiguration());
        modelBuilder.ApplyConfiguration(new FileGroupConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TemporaryLinkConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration["NpgsqlConnectionString"]);
        
        base.OnConfiguring(optionsBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (BaseEntity entity in ChangeTracker.Entries()
                     .Where(x => x.Entity is BaseEntity)
                     .Select(x => x.Entity))
        {
            entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}