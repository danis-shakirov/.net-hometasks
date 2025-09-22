using API.DTOs;
using API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
            return BadRequest("Поле login обязательно");

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Поле password обязательно");

            
        var result = await authService.Login(request.Login, request.Password);
        return result
            ? Ok(new { expires_in = 3600 })
            : Unauthorized("Неверный логин или пароль");
    }

    public async Task<IActionResult> Logout()
    {
        await authService.Logout();

        return Ok();
    }

    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
            return BadRequest("Поле login обязательно");

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Поле password обязательно");

        if (request.Age <= 0)
            return BadRequest("Поле age должно быть больше 0");
            
        var newUser = new UserDto(
            Id: Guid.NewGuid(),
            Login: request.Login,
            Password: request.Password,
            Age: request.Age);
            
        var result = await authService.Register(newUser);
        return result
            ? Created($"/users/{newUser.Id}", new { newUser.Id, request.Login, request.Age })
            : Conflict("Пользователь с таким логином уже существует");
    }
}