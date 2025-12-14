using API.Filters;
using BLL.Interfaces.Auth;
using Domain.Requests;
using Domain.WebExceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Tags("Auth")]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest dto)
    {
        try
        {
            var result = await authService.LoginAsync(dto.Login, dto.Password);
            
            return Ok(new {accessToken = result});
        }
        catch (UnauthorizedException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            logger.LogError("Error during login: {exWithStack}", ex);
            
            return Problem("Something went wrong");
        }
    }

    [HttpPost("register")]
    [ServiceFilter(typeof(ValidationFilter<RegisterRequest>))]
    public async Task<IActionResult> Register(RegisterRequest dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Login))
                return BadRequest("Поле login обязательно");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Поле password обязательно");

            if (dto.Age <= 0)
                return BadRequest("Поле age должно быть больше 0");

            var newUserId = await authService.RegisterAsync(username: dto.Login, password: dto.Password, age: dto.Age);
            return Created($"/users/{newUserId}", new { newUserId, dto.Login, dto.Age });
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError("Error during register: {exWithStack}", ex);
            return Problem("Something went wrong");
        }
    }
}