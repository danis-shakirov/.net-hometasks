using Domain.Models;

namespace BLL.Interfaces.Auth;

public interface IAuthService
{
    Task<string> LoginAsync(string username, string password);
    Task<Guid> RegisterAsync(string username, string password, int age);
}