using FileStorage.Application.Models;
using FileStorage.Domain.Entities;
using Microsoft.AspNetCore.Http;
using File = FileStorage.Domain.Entities.File;

namespace FileStorage.Application.Interfaces;

/// <summary>
/// Сервис для работы с файлами.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Загрузить файлы.
    /// </summary>
    /// <param name="files">Коллекция файлов.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Идентификатор загруженного файла</returns>
    public Task<Guid> UploadFilesAsync(IEnumerable<IFormFile> files, Guid userId, CancellationToken token);

    /// <summary>
    /// Получить файл (сущность).
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<File> GetFileAsync(Guid fileId, Guid userId, CancellationToken token);

    /// <summary>
    /// Получить группу файлов (сущность).
    /// </summary>
    /// <param name="fileGroupId">Идентификатор группы файлов.</param>
    /// <param name="userId"></param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<FileGroup> GetFileGroupAsync(Guid fileGroupId, Guid userId, CancellationToken token);

    /// <summary>
    /// Получить файл (физический).
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="userId"></param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<FileMetadata> GetPhysicalFileAsync(Guid fileId, Guid userId, CancellationToken token);

    /// <summary>
    /// Получить группу файлов.
    /// </summary>
    /// <param name="fileGroupId">Идентификатор группы файлов.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Коллекция <see cref="FileMetadata"/></returns>
    public Task<IEnumerable<FileMetadata>> GetPhysicalFilesAsync(Guid fileGroupId, Guid userId,
        CancellationToken token);

    /// <summary>
    /// Получить список загруженных групп файлов пользователем.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<IEnumerable<FileGroup>> GetFileGroupsByUserIdAsync(Guid userId, CancellationToken token);

    /// <summary>
    /// Получить список загруженных файлов пользователем.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<IEnumerable<File>> GetFilesByUserIdAsync(Guid userId, CancellationToken token);

    /// <summary>
    /// Получить список файлов в группе файлов.
    /// </summary>
    /// <param name="fileGroupId">Идентификатор группы файлов.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<IEnumerable<File?>> GetFilesByFileGroupIdAsync(Guid fileGroupId, Guid userId, CancellationToken token);

    /// <summary>
    /// Получить одноразовую ссылку на файл.
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<string> GetTemporaryLinkForFileAsync(Guid fileId, Guid userId, CancellationToken token);

    /// <summary>
    /// Получить одноразовую ссылку на группу файлов.
    /// </summary>
    /// <param name="fileGroupId">Идентификатор группы файлов.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<string> GetTemporaryLinkForFileGroupAsync(Guid fileGroupId, Guid userId, CancellationToken token);

    /// <summary>
    /// Получить файл по одноразовой ссылке.
    /// </summary>
    /// <param name="temporaryLinkId">Уникальный номер</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<FileMetadata> GetFileByTemporaryLink(Guid temporaryLinkId, CancellationToken token);

    /// <summary>
    /// Получить группу файлов по одноразовой ссылке.
    /// </summary>
    /// <param name="temporaryLinkId">Уникальный номер</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<IEnumerable<FileMetadata>> GetFilesByTemporaryLink(Guid temporaryLinkId, CancellationToken token);

    /// <summary>
    /// Получить сущность временной ссылки по идентификатору.
    /// </summary>
    /// <param name="temporaryLinkId">Идентификатор временной ссылки</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    public Task<TemporaryLink> GetTemporaryLinkByIdAsync(Guid temporaryLinkId, CancellationToken token);

    /// <summary>
    /// Получить процент загрузки файла
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Число 0-100%</returns>
    public Task<decimal> GetFileProgressAsync(Guid fileId, Guid userId, CancellationToken token);

    /// <summary>
    /// Получить процент загрузки группы файлов
    /// </summary>
    /// <param name="fileGroupId">Идентификатор группы файлов</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Число 0-100%</returns>
    public Task<decimal> GetFileGroupProgressAsync(Guid fileGroupId, Guid userId, CancellationToken token);
}