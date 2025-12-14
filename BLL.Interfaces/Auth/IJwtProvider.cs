using Domain.Models;

namespace BLL.Interfaces.Auth;

public interface IJwtProvider
{
    string GenerateToken(User user);
}