using FileStorage.Domain.Common;

namespace FileStorage.Domain.Entities;

/// <summary>
/// Группа файлов.
/// </summary>
public class FileGroup : BaseEntity
{
    /// <summary>
    /// Коллекция файлов группы.
    /// </summary>
    public IList<File> Files { get; init; } = new List<File>();

    /// <summary>
    /// Идентификатор пользователя, загрузившего файлы.
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    /// Cущность пользователя, загрузившего файлы.
    /// </summary>
    public User User { get; set; } = null!;
}


