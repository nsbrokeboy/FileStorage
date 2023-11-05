using System.IO.Compression;
using FileStorage.Application.Interfaces;
using FileStorage.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FileStorage.Infrastructure.Services;

public class StorageService : IStorageService
{
    private readonly string _basePath;

    public StorageService(IConfiguration configuration)
    {
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), configuration["BaseDirectoriesNames:DataFolder"],
            configuration["BaseDirectoriesNames:UploadsFolder"]);
    }

    public async Task<byte[]> LoadFileFromDisk(CancellationToken token, params string[] filePath)
    {
        var path = GetCombinedPath(filePath);
        await using var fileStream = new FileStream(path, FileMode.Open);

        int read;
        var buffer = new byte[16 * 1024];
        await using var memoryStream = new MemoryStream();

        while ((read = await fileStream.ReadAsync(buffer, token)) > 0)
        {
            memoryStream.Write(buffer, 0, read);
        }

        return memoryStream.ToArray();
    }

    public async Task SaveFileOnDisk(IFormFile file, CancellationToken token, params string[] filePath)
    {
        var path = GetCombinedPath(filePath);

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream, token);
    }

    public DirectoryInfo CreateDirectoryForFileGroup(params string[] filePath)
    {
        var path = GetCombinedPath(filePath);
        return Directory.CreateDirectory(path);
    }

    private string GetCombinedPath(params string[] filePath)
    {
        var innerPath = filePath.Aggregate(Path.Combine);
        return Path.Combine(_basePath, innerPath);
    }

    public async Task<FileMetadata> CreateZipArchive(IEnumerable<FileMetadata> files, CancellationToken token,
        string archiveName = "archive")
    {
        await using var memoryStream = new MemoryStream();
        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var entry = zipArchive.CreateEntry(file.FileName);
                await using var entryStream = entry.Open();
                await entryStream.WriteAsync(file.Content.AsMemory(0, file.Content.Length), token);
            }
        }

        var zipContent = memoryStream.ToArray();
        var zipFileMetadata = new FileMetadata
        {
            FileName = $"{archiveName}.zip",
            ContentType = "application/zip",
            Content = zipContent
        };
        return zipFileMetadata;
    }
}