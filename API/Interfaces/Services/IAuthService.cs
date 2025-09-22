using API.DTOs;

namespace API.Interfaces.Services;

public interface IAuthService
{
    Task<bool> Login(string login, string password);
    Task Logout();
    Task<bool> Register(UserDto userDto);
}