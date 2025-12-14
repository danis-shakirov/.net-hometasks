using System.Security.Claims;
using BLL.Interfaces.Services;
using Domain.Enums;
using Domain.Models;
using Domain.WebExceptions;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await userService.GetUserAsync(id);
        return user != null
            ? Ok(user)
            : NotFound();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetUsers()
        => Ok(await userService.GetUsersAsync());

    [HttpGet("Created")]
    public async Task<IActionResult> GetUsersCreatedByDate(DateTime creationDate, bool laterThanDate)
        => Ok(await userService.GetUsersCreatedByDateAsync(creationDate, laterThanDate));

    [HttpGet("Updated")]
    public async Task<IActionResult> GetUsersUpdatedByDate(DateTime updateDate, bool laterThanDate)
        => Ok(await userService.GetUsersUpdatedByDateAsync(updateDate, laterThanDate));

    [HttpGet("Created/Between")]
    public async Task<ActionResult<ICollection<UserDto>>> GetCreatedBetween([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        if (start is null || end is null)
            return BadRequest("Query-параметры 'start' и 'end' обязательны.");

        var users = await userService.GetUsersCreatedBetweenAsync(start.Value, end.Value);
        return Ok(users);
    }

    [HttpGet("Updated/Between")]
    public async Task<ActionResult<ICollection<UserDto>>> GetUpdatedBetween([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        if (start is null || end is null)
            return BadRequest("Query-параметры 'start' и 'end' обязательны.");

        var users = await userService.GetUsersUpdatedBetweenAsync(start.Value, end.Value);
        return Ok(users);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserDto dto)
    {
        try
        {
            Guid? userId = Guid.TryParse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)
                ? id
                : null;
            
            Role? userRole = Enum.TryParse<Role>(HttpContext.User.FindFirst(ClaimTypes.Role)?.Value, out var role)
                ? role
                : null;
            
            if (userId is null)
            {
                return Unauthorized();
            }
            
            if (userId != dto.Id && role != Role.Admin)
            {
                return Forbid();
            }

            var user = mapper.Map<User>(dto);

            await userService.UpdateUserAsync(user);

            return Ok();
        }
        catch (NotFoundException)
        {
            return NotFound($"User with id {dto.Id} not found");
        }
    }
}