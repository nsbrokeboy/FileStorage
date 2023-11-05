namespace FileStorage.Application.Models;

/// <summary>
/// Выходной файл.
/// </summary>
public class FileMetadata
{
    /// <summary>
    /// Имя файла.
    /// </summary>
    public string FileName { get; init; } = null!;
    
    /// <summary>
    /// Content-Type.
    /// </summary>
    public string ContentType { get; init; } = null!;
    
    /// <summary>
    /// blob файла.
    /// </summary>
    public byte[] Content { get; init; } = Array.Empty<byte>();
}