using System.Net;
using System.Text.Json;
using FileStorage.Application.Common.Exceptions;
using UnauthorizedAccessException = FileStorage.Application.Common.Exceptions.UnauthorizedAccessException;

namespace FileStorage.Api.Middlewares;

/// <summary>
/// Мидлвейр для перехвата ошибок и возвращения корректных кодов ответа
/// </summary>
public class CustomExceptionsHandler
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Конструктор
    /// </summary>
    public CustomExceptionsHandler(RequestDelegate next)
    {
        _next = next;
    }
    
    /// <summary>
    /// Перехват
    /// </summary>
    /// <param name="context"></param>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }
 
    /// <summary>
    /// Основная обработка ошибок
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="exception">Исключение</param>
    /// <returns></returns>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        string result;
 
        switch (exception)
        {
            case NotFoundException notFoundException:
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(notFoundException.Message);
                break;
                
            case UnauthorizedAccessException unauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(unauthorizedAccessException.Message);
                break;
                
            // для других исключений
            default:
                result = "Internal server error";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }
 
        context.Response.ContentType = "application/json";
 
        context.Response.StatusCode = (int)code;
 
        if (result == "null")
        {
            result = JsonSerializer.Serialize(new { error = exception.Message });
        }
 
        return context.Response.WriteAsync(result);
    }
}