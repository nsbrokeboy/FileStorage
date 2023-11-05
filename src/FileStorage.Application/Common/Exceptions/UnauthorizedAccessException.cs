namespace FileStorage.Application.Common.Exceptions;

/// <summary>
/// Исключение, возникающее при запросе ресурса, к которому у пользователя нет доступа.
/// </summary>
public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException(string? message) : base(message)
    {
    }
}