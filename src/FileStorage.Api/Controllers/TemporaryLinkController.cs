using FileStorage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Api.Controllers;

/// <summary>
/// Контроллер для одноразовых ссылок на файлы.
/// </summary>
[Route("temp")]
public class TemporaryLinkController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IStorageService _storageService;

    /// <summary>
    /// Конструктор класса <see cref="TemporaryLinkController"/>.
    /// </summary>
    /// <param name="fileService">Сервис для работы с файлами в приложении</param>
    /// <param name="storageService">Сервис для работы с физическими файлами</param>
    public TemporaryLinkController(IFileService fileService, IStorageService storageService)
    {
        _fileService = fileService;
        _storageService = storageService;
    }

    /// <summary>
    /// Создать одноразовый ссылку на файл.
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Ссылка на файл</returns>
    /// <response code="200">Успешный запрос</response>
    /// <response code="401">Нет доступа к запрашиваемому ресурсу</response>
    /// <response code="404">Указанный ресурс не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpPost("create/file/{fileId}")]
    public async Task<IActionResult> CreateTemporaryLinkForFileAsync(Guid fileId, Guid userId, CancellationToken token)
    {
        var uniqueLink = await _fileService.GetTemporaryLinkForFileAsync(fileId, userId, token);
        var link = HttpContext.Request.Scheme + "://" +
                   string.Join("/", HttpContext.Request.Host.ToString(), "temp", "download", "file", uniqueLink);

        return Ok(link);
    }

    /// <summary>
    /// Создать одноразовый ссылку на группу файлов.
    /// </summary>
    /// <param name="fileGroupId">Идентификатор группы файлов</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Одноразовая ссылка</returns>
    /// <response code="200">Успешный запрос</response>
    /// <response code="401">Нет доступа к запрашиваемому ресурсу</response>
    /// <response code="404">Указанный ресурс не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpPost("create/files/{fileGroupId}")]
    public async Task<IActionResult> CreateTemporaryLinkForFileGroupAsync(Guid fileGroupId, Guid userId, CancellationToken token)
    {
        var uniqueLink = await _fileService.GetTemporaryLinkForFileGroupAsync(fileGroupId, userId, token);
        var link = HttpContext.Request.Scheme + "://" +
                   string.Join("/", HttpContext.Request.Host.ToString(), "temp", "download", "files", uniqueLink);

        return Ok(link);
    }

    /// <summary>
    /// Скачать файл по одноразовой ссылке.
    /// </summary>
    /// <param name="tempLinkId">Уникальный идентификатор</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Файл</returns>
    /// <response code="200">Успешный запрос</response>
    /// <response code="401">Нет доступа к запрашиваемому ресурсу</response>
    /// <response code="404">Указанный ресурс не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpGet("download/file/{tempLinkId}")]
    public async Task<IActionResult> DownloadFileByTempLink(Guid tempLinkId, CancellationToken token)
    {
        var file = await _fileService.GetFileByTemporaryLink(tempLinkId, token);
        
        return File(file.Content, file.ContentType, file.FileName);
    }

    /// <summary>
    /// Скачать группу файлов по одноразовой ссылке.
    /// </summary>
    /// <param name="tempLinkId">Уникальный идентификатор</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Архив с файлами</returns>
    /// <response code="200">Успешный запрос</response>
    /// <response code="401">Нет доступа к запрашиваемому ресурсу</response>
    /// <response code="404">Указанный ресурс не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpGet("download/files/{tempLinkId}")]
    public async Task<IActionResult> DownloadFileGroupByTempLink(Guid tempLinkId, CancellationToken token)
    {
        var files = await _fileService.GetFilesByTemporaryLink(tempLinkId, token);
        
        var zip = await _storageService.CreateZipArchive(files, token, tempLinkId.ToString());
        return File(zip.Content, zip.ContentType, zip.FileName);
    }
}