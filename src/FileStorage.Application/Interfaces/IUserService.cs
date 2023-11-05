using FileStorage.Domain.Entities;

namespace FileStorage.Application.Interfaces;

/// <summary>
/// Сервис для работы с пользователями.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Создать пользователя.
    /// </summary>
    /// <returns>Идентификатор созданного пользователя.</returns>
    public Task<Guid> CreateUserAsync(CancellationToken token);
    
    /// <summary>
    /// Получить пользователя по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="token">token</param>
    /// <returns>Сущность <see cref="User"/></returns>
    public Task<User> GetUserByIdAsync(Guid id, CancellationToken token);
}