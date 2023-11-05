using FileStorage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Api.Controllers;

/// <summary>
/// Контроллер для работы с файлами.
/// </summary>
[ApiController]
[Route("files")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IStorageService _storageService;

    /// <summary>
    /// Конструктор класса <see cref="FileController"/>.
    /// </summary>
    /// <param name="fileService">Сервис для работы с файлами в приложении</param>
    /// <param name="storageService">Сервис для работы с физическими файлами</param>
    public FileController(IFileService fileService, IStorageService storageService)
    {
        _fileService = fileService;
        _storageService = storageService;
    }

    /// <summary>
    /// Загрузить файл в хранилище.
    /// </summary>
    /// <param name="files">Коллекция файлов</param>
    /// <param name="userId">Идентификатор пользователя, загружающего файлы</param>
    /// <param name="token">CancellationToken</param>
    /// <response code="200">Успешный запрос</response>
    /// <response code="404">Указанный пользователь не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpPost("files/upload")]
    public async Task<IActionResult> UploadFiles(IEnumerable<IFormFile> files, Guid userId, CancellationToken token)
    {
        return Ok(await _fileService.UploadFilesAsync(files, userId, token));
    }

    /// <summary>
    /// Получить список файлов пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <response code="200">Успешный запрос</response>
    /// <response code="404">Указанный пользователь не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpGet("file/list/{userId}")]
    public async Task<IActionResult> GetListOfUploadedFiles(Guid userId, CancellationToken token)
    {
        return Ok(await _fileService.GetFilesByUserIdAsync(userId, token));
    }

    /// <summary>
    /// Получить список групп файлов пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <response code="200">Успешный запрос</response>
    /// <response code="404">Указанный пользователь не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpGet("files/list/{userId}")]
    public async Task<IActionResult> GetListOfUpdatedGroupOfFiles(Guid userId, CancellationToken token)
    {
        return Ok(await _fileService.GetFileGroupsByUserIdAsync(userId, token));
    }

    /// <summary>
    /// Получить список файлов пользователя по группе.
    /// </summary>
    /// <param name="groupId">Идентификатор файла</param>
    /// <param name="userId">Идентификатор группы</param>
    /// <param name="token">CancellationToken</param>
    /// <response code="200">Успешный запрос</response>
    /// <response code="401">Нет доступа к запрашиваемому ресурсу</response>
    /// <response code="404">Указанный пользователь не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpGet("files/list/{userId}/{groupId}")]
    public async Task<IActionResult> GetFilesInGroup(Guid groupId, Guid userId, CancellationToken token)
    {
        return Ok(await _fileService.GetFilesByFileGroupIdAsync(groupId, userId, token));
    }

    /// <summary>
    /// Скачать файл из хранилища.
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Файл</returns>
    /// <response code="200">Успешный запрос</response>
    /// <response code="401">Нет доступа к запрашиваемому ресурсу</response>
    /// <response code="404">Указанный ресурс не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpGet("file/download/{fileId}")]
    public async Task<IActionResult> DownloadFile(Guid fileId, Guid userId, CancellationToken token)
    {
        var file = await _fileService.GetPhysicalFileAsync(fileId, userId, token);

        return File(file.Content, file.ContentType, file.FileName);
    }

    /// <summary>
    /// Скачать группу файлов из хранилища.
    /// </summary>
    /// <param name="fileGroupId">Идентификатор группы файлов</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Zip-архив с файлами</returns>
    /// <response code="200">Успешный запрос</response>
    /// <response code="401">Нет доступа к запрашиваемому ресурсу</response>
    /// <response code="404">Указанный ресурс не найден</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpGet("files/download/{fileGroupId}")]
    public async Task<IActionResult> DownloadFiles(Guid fileGroupId, Guid userId, CancellationToken token)
    {
        var files = await _fileService.GetPhysicalFilesAsync(fileGroupId, userId, token);
        var zip = await _storageService.CreateZipArchive(files, token, fileGroupId.ToString());

        return File(zip.Content, zip.ContentType, zip.FileName);
    }

    /// <summary>
    /// Получить процент загрузки файла.
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    [HttpGet("file/progress/{fileId}/")]
    public async Task<IActionResult> GetFileProgress(Guid fileId, Guid userId, CancellationToken token)
    {
        var progress = await _fileService.GetFileProgressAsync(fileId, userId, token);

        return Ok(progress);
    }

    /// <summary>
    /// Получить процент загрузки группы файлов.
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="token">CancellationToken</param>
    /// <returns></returns>
    [HttpGet("files/progress/{fileGroupId}/")]
    public async Task<IActionResult> GetFileGroupProgress(Guid fileGroupId, Guid userId, CancellationToken token)
    {
        var progress = await _fileService.GetFileGroupProgressAsync(fileGroupId, userId, token);

        return Ok(progress);
    }
}