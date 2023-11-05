namespace FileStorage.Domain.Common;

/// <summary>
///  Базовый класс для всех доменных моделей.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Дата и время создания.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow; 
    
    /// <summary>
    /// Дата и время последнего изменения.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; 

    /// <summary>
    /// Флаг активной записи.
    /// </summary>
    public bool IsActive { get; set; } = true;
}