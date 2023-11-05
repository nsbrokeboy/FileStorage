using FileStorage.Application.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorage.Application.Interfaces;

/// <summary>
/// Сервис для райботы с файлами на диске.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Получить файл по пути.
    /// </summary>
    /// <param name="token">CancellationToken</param>
    /// <param name="filePath">Составной путь</param>
    /// <returns>blob файла</returns>
    public Task<byte[]> LoadFileFromDisk(CancellationToken token, params string[] filePath);

    /// <summary>
    /// Сохранить файл на диске.
    /// </summary>
    /// <param name="file">Файл</param>
    /// <param name="token">CancellationToken</param>
    /// <param name="filePath">Составной путь</param>
    /// <returns></returns>
    public Task SaveFileOnDisk(IFormFile file, CancellationToken token, params string[] filePath);
    
    /// <summary>
    /// Создать директорию под группу файлов.
    /// </summary>
    /// <param name="filePath">Составной путь</param>
    /// <returns>Объект <see cref="DirectoryInfo"/></returns>
    public DirectoryInfo CreateDirectoryForFileGroup(params string[] filePath);

    /// <summary>
    /// Запаковать файлы в архив.
    /// </summary>
    /// <param name="files">Файлы</param>
    /// <param name="token">CreateZipArchive</param>
    /// <param name="archiveName">Имя выходного архива</param>
    /// <returns>Архив в виде объекта <see cref="FileMetadata"/></returns>
    public Task<FileMetadata> CreateZipArchive(IEnumerable<FileMetadata> files,
        CancellationToken token,
        string archiveName);
}