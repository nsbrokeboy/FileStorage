using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = FileStorage.Domain.Entities.File;

namespace FileStorage.Infrastructure.Data.Configurations;

public class FileConfiguration : BaseEntityConfiguration<File>
{
    public override void Configure(EntityTypeBuilder<File> builder)
    {
        builder.Property(x => x.OriginalFilename).HasMaxLength(255);
        builder.Property(x => x.ContentType).HasMaxLength(255);
        
        base.Configure(builder);
    }
}