using BLL.Interfaces;
using BLL.Interfaces.Auth;
using Domain.Enums;
using Domain.Models;
using Domain.WebExceptions;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementation.Auth;

public class AuthService(
    IUnitOfWork uow,
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher) : IAuthService
{
    public async Task<string> LoginAsync(string username, string password)
    {
        var user = await uow.Users.FirstOrDefaultAsync(u => u.Login == username);
        if (user == null || !passwordHasher.Verify(password, user.Password))
            throw new UnauthorizedException();
        
        return jwtProvider.GenerateToken(user);
    }

    public async Task<Guid> RegisterAsync(string username, string password, int age)
    {
        var existedUser = await uow.Users.FirstOrDefaultAsync(u => u.Login == username);
        if (existedUser != null)
            throw new ConflictException("Username already taken");

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Login = username,
            Password = passwordHasher.Generate(password),
            Age = age,
            Role = Role.User,
            CreationDate = DateTime.Now.ToUniversalTime(),
            UpdateDate = DateTime.Now.ToUniversalTime(),
        };
        
        await uow.Users.AddAsync(newUser);
        await uow.SaveChangesAsync();
        
        return newUser.Id;
    }
}