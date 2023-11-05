using FileStorage.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Api.Controllers;

/// <summary>
/// Контроллер для пользователей
/// </summary>
[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="userService"></param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Создать пользователя
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Guid созданного пользователя</returns>
    /// <response code="200">Успешный запрос</response>
    /// <response code="500">Ошибка при обработке запроса</response>
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateUser(CancellationToken token)
    {
        return Ok(await _userService.CreateUserAsync(token));
    }
}