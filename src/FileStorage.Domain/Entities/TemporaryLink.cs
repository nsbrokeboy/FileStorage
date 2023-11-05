using FileStorage.Domain.Common;

namespace FileStorage.Domain.Entities;

/// <summary>
/// Временная ссылка на файл.
/// </summary>
public class TemporaryLink : BaseEntity
{
    /// <summary>
    /// Идентификатор файла.
    /// </summary>
    public Guid? FileId { get; init; }
    
    /// <summary>
    /// Сущность файла.
    /// </summary>
    public File? File { get; init; } 

    /// <summary>
    /// Идентификатор группы файлов.
    /// </summary>
    public Guid? FileGroupId { get; init; }

    /// <summary>
    /// Сущность группы файлов.
    /// </summary>
    public FileGroup? FileGroup { get; init; }
}