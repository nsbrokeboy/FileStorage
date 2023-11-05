using FileStorage.Application.Common.Exceptions;
using FileStorage.Application.Interfaces;
using FileStorage.Application.Models;
using FileStorage.Domain.Entities;
using FileStorage.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using File = FileStorage.Domain.Entities.File;
using UnauthorizedAccessException = FileStorage.Application.Common.Exceptions.UnauthorizedAccessException;

namespace FileStorage.Infrastructure.Services;

/// <inheritdoc /> 
public class FileService : IFileService
{
    private readonly FileStorageDbContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IUserService _userService;

    public FileService(FileStorageDbContext dbContext, IStorageService storageService, IUserService userService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
        _userService = userService;
    }

    /// <inheritdoc />
    public async Task<Guid> UploadFilesAsync(IEnumerable<IFormFile> files, Guid userId,
        CancellationToken token)
    {
        var user = await _userService.GetUserByIdAsync(userId, token);

        var fileGroup = new FileGroup
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Files = new List<File>()
        };
        await _dbContext.FileGroups.AddAsync(fileGroup, token);

        var directory = _storageService.CreateDirectoryForFileGroup(user.Id.ToString(), fileGroup.Id.ToString());

        foreach (var file in files)
        {
            var uploadedFile = new File
            {
                Id = Guid.NewGuid(),
                OriginalFilename = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length
            };
            _dbContext.Files.Add(uploadedFile);

            await _storageService.SaveFileOnDisk(file, token,
                directory.ToString(), uploadedFile.Id.ToString());

            fileGroup.Files.Add(uploadedFile);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        return fileGroup.Id;
    }

    /// <inheritdoc />
    public async Task<File> GetFileAsync(Guid fileId, Guid userId, CancellationToken token)
    {
        var user = await _userService.GetUserByIdAsync(userId, token);

        var file = await _dbContext.Files
            .Include(f => f.FileGroup)
            .SingleOrDefaultAsync(f => f.Id == fileId, token);
        if (file == null)
        {
            throw new NotFoundException("Файл не найден");
        }

        if (file.FileGroup.UserId != user.Id)
        {
            throw new UnauthorizedAccessException("У вас нет прав доступа к данному файлу");
        }

        return file;
    }

    /// <inheritdoc />
    public async Task<FileGroup> GetFileGroupAsync(Guid fileGroupId, Guid userId, CancellationToken token)
    {
        var user = await _userService.GetUserByIdAsync(userId, token);

        var fileGroup = await _dbContext.FileGroups
            .Include(fg => fg.Files)
            .SingleOrDefaultAsync(f => f.Id == fileGroupId, token);
        
        if (fileGroup == null)
        {
            throw new NotFoundException("Группа файлов не найдена");
        }

        if (fileGroup.UserId != user.Id)
        {
            throw new UnauthorizedAccessException("У вас нет прав доступа к данной группе файлов");
        }

        return fileGroup;
    }

    /// <inheritdoc />
    public async Task<FileMetadata> GetPhysicalFileAsync(Guid fileId, Guid userId, CancellationToken token)
    {
        var file = await GetFileAsync(fileId, userId, token);

        var content = await _storageService.LoadFileFromDisk(token, userId.ToString(),
            file.FileGroupId.ToString(), file.Id.ToString());
        return new FileMetadata
        {
            FileName = file.OriginalFilename,
            ContentType = file.ContentType,
            Content = content
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FileMetadata>> GetPhysicalFilesAsync(Guid fileGroupId, Guid userId,
        CancellationToken token)
    {
        var files = await GetFilesByFileGroupIdAsync(fileGroupId, userId, token);

        var physicalFiles = new List<FileMetadata>();
        foreach (var file in files)
        {
            physicalFiles.Add(await GetPhysicalFileAsync(file!.Id, userId, token));
        }

        return physicalFiles;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FileGroup>> GetFileGroupsByUserIdAsync(Guid userId,
        CancellationToken token) =>
        await _dbContext.FileGroups
            .Where(fg => fg.UserId == userId)
            .ToListAsync(token);

    /// <inheritdoc />
    public async Task<IEnumerable<File>> GetFilesByUserIdAsync(Guid userId, CancellationToken token)
        => await _dbContext.Files
            .Where(f => f.FileGroup.UserId == userId)
            .ToListAsync(token);

    /// <inheritdoc />
    public async Task<IEnumerable<File?>> GetFilesByFileGroupIdAsync(Guid fileGroupId, Guid userId,
        CancellationToken token)
    {
        var fileGroup = await GetFileGroupAsync(fileGroupId, userId, token);
        
        return fileGroup.Files;
    }

    /// <inheritdoc />
    public async Task<string> GetTemporaryLinkForFileAsync(Guid fileId, Guid userId,
        CancellationToken token)
    {
        var file = await GetFileAsync(fileId, userId, token);

        var link = new TemporaryLink
        {
            File = file,
        };
        await _dbContext.TemporaryLinks.AddAsync(link, token);
        await _dbContext.SaveChangesAsync(token);
        return link.Id.ToString();
    }

    /// <inheritdoc />
    public async Task<string> GetTemporaryLinkForFileGroupAsync(Guid fileGroupId, Guid userId,
        CancellationToken token)
    {
        var fileGroup = await GetFileGroupAsync(fileGroupId, userId, token);

        var link = new TemporaryLink
        {
            FileGroup = fileGroup
        };
        
        await _dbContext.TemporaryLinks.AddAsync(link, token);
        await _dbContext.SaveChangesAsync(token);
        return link.Id.ToString();
    }

    /// <inheritdoc />
    public async Task<FileMetadata> GetFileByTemporaryLink(Guid temporaryLinkId,
        CancellationToken token)
    {
        // TODO: может быть конкуррентный доступ к ссылке - нужен лок
        var temporaryLink = await GetTemporaryLinkByIdAsync(temporaryLinkId, token);

        var file = await _dbContext.Files
            .Include(f => f.FileGroup)
            .SingleOrDefaultAsync(f => f.Id == temporaryLink.FileId, token);
        if (file == null)
        {
            throw new NotFoundException("Файл не найден");
        }

        var content = await _storageService.LoadFileFromDisk(token,
            file.FileGroup.UserId.ToString(),
            file.FileGroup.Id.ToString(),
            file.Id.ToString());

        temporaryLink.IsActive = false;
        await _dbContext.SaveChangesAsync(token);
        
        return new FileMetadata
        {
            FileName = file.OriginalFilename,
            ContentType = file.ContentType,
            Content = content
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FileMetadata>> GetFilesByTemporaryLink(Guid temporaryLinkId,
        CancellationToken token)
    {
        // TODO: может быть конкуррентный доступ к ссылке - нужен лок
        var temporaryLink = await GetTemporaryLinkByIdAsync(temporaryLinkId, token);

        var fileGroup =
            await _dbContext.FileGroups.SingleOrDefaultAsync(f => f.Id == temporaryLink.FileGroupId, token);
        if (fileGroup == null)
        {
            throw new NotFoundException("Группа файлов не найдена");
        }
        
        var files = await GetFilesByFileGroupIdAsync(fileGroup.Id, fileGroup.UserId, token);
        var physicalFiles = new List<FileMetadata>();
        foreach (var file in files)
        {
            physicalFiles.Add(await GetPhysicalFileAsync(file!.Id, fileGroup.UserId, token));
        }

        return physicalFiles;
    }

    /// <inheritdoc />
    public async Task<TemporaryLink> GetTemporaryLinkByIdAsync(Guid temporaryLinkId,
        CancellationToken token)
    {
        var temporaryLink =
            await _dbContext.TemporaryLinks.SingleOrDefaultAsync(t => t.Id == temporaryLinkId, token);

        if (temporaryLink == null)
        {
            throw new NotFoundException("Временная ссылка не найдена");
        }

        if (!temporaryLink.IsActive)
        {
            throw new UnauthorizedAccessException("Временная ссылка уже использована");
        }

        return temporaryLink;
    }
}