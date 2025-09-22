using API.DTOs;
using API.Filters;
using API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(RequireUserIdFilter))]
public class UsersController(IUserService userService) : ControllerBase
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
    public async Task<IActionResult> UpdateUser(UserDto user)
    {
        var userId = HttpContext.Items["UserId"] as Guid?;
        if (userId != user.Id)
        {
            return Forbid();
        }

        return Ok(await userService.UpdateUserAsync(user));
    }
}