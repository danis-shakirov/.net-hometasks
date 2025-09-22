using API.DTOs;
using API.Interfaces.Repositories;
using API.Interfaces.Services;

namespace API.Services;

public class AuthService(IUserRepository userRepository, ISessionService sessionService) : IAuthService
{
    public async Task<bool> Login(string login, string password)
    {
        var user = await userRepository.GetUserByCredentialsAsync(login, password);
        if (user is null)
            return false;

        sessionService.CreateSession(user.Id);

        return true;
    }

    public async Task Logout() => sessionService.DestroySession();

    public async Task<bool> Register(UserDto userDto)
    {
        var existingUser = await userRepository.GetUserByLogin(userDto.Login);
        if (existingUser != null) 
            return false;
        
        await userRepository.SaveUserAsync(userDto);
        return true;
    }
}