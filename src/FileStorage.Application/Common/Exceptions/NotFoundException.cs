namespace FileStorage.Application.Common.Exceptions;

/// <summary>
/// Исключение, возникающее при отсутствии ресурса
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string? message) : base(message)
    {
    }
}