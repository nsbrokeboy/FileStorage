using FileStorage.Domain.Common;

namespace FileStorage.Domain.Entities;

/// <summary>
/// Файл.
/// </summary>
public class File : BaseEntity
{
    /// <summary>
    /// Исходное имя файла.
    /// </summary>
    public string OriginalFilename { get; init; } = null!;
    
    /// <summary>
    /// Размер файла.
    /// </summary>
    public long Size { get; init; }
    
    /// <summary>
    /// Content-Type.
    /// </summary>
    public string ContentType { get; init; } = null!;

    /// <summary>
    /// Идентификатор группы файлов.
    /// </summary>
    public Guid FileGroupId { get; init; }
    
    /// <summary>
    /// Сущность группы файлов.
    /// </summary>
    public FileGroup FileGroup { get; set; } = null!;
}