using System.Collections.Concurrent;
using API.DTOs;
using API.Entities;
using API.Interfaces.Repositories;

namespace API.Repositories;

public class UserRepository(DbContextMock db) : IUserRepository
{
    private static UserDto? ToDto(UserEntity? e) =>
        e is null ? null : new UserDto(e.Id, e.Login, e.Password, e.Age);

    private static UserEntity ToEntity(UserDto d, DateTime? created = null, DateTime? updated = null) =>
        new()
        {
            Id = d.Id,
            Login = d.Login,
            Password = d.Password,
            Age = d.Age,
            CreationDate = created ?? DateTime.UtcNow,
            UpdateDate = updated ?? DateTime.UtcNow
        };
    
    public Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var entity = db.Users.FirstOrDefault(u => u.Id == id);
        
        return Task.FromResult(ToDto(entity));
    }

    public Task<UserDto?> GetUserByLogin(string login)
    {
        var entity = db.Users.FirstOrDefault(u =>
            string.Equals(u.Login, login, StringComparison.Ordinal));
        
        return Task.FromResult(ToDto(entity));
    }

    public Task<UserDto?> GetUserByCredentialsAsync(string login, string password)
    {
        var entity = db.Users.FirstOrDefault(u =>
            string.Equals(u.Login, login, StringComparison.Ordinal) &&
            string.Equals(u.Password, password, StringComparison.Ordinal));
        
        return Task.FromResult(ToDto(entity));
    }

    public Task<ICollection<UserDto>> GetUsersAsync()
    {
        var list = db.Users.Select(u => ToDto(u)!).ToList();
        
        return Task.FromResult<ICollection<UserDto>>(list);
    }

    public Task<ICollection<UserDto>> GetUsersCreatedByDateAsync(DateTime creationDate, bool laterThanDate)
    {
        var q = laterThanDate
            ? db.Users.Where(u => u.CreationDate >= creationDate)
            : db.Users.Where(u => u.CreationDate <= creationDate);

        var list = q.Select(u => ToDto(u)!).ToList();
        return Task.FromResult<ICollection<UserDto>>(list);
    }

    public Task<ICollection<UserDto>> GetUsersUpdatedByDateAsync(DateTime updateDate, bool laterThanDate)
    {
        var q = laterThanDate
            ? db.Users.Where(u => u.UpdateDate >= updateDate)
            : db.Users.Where(u => u.UpdateDate <= updateDate);

        var list = q.Select(u => ToDto(u)!).ToList();
        return Task.FromResult<ICollection<UserDto>>(list);
    }
    
    public Task<ICollection<UserDto>> GetUsersCreatedBetweenAsync(DateTime start, DateTime end)
    {
        var list = db.Users
            .Where(u => u.CreationDate >= start && u.CreationDate <= end)
            .Select(u => ToDto(u)!)
            .ToList();

        return Task.FromResult<ICollection<UserDto>>(list);
    }

    public Task<ICollection<UserDto>> GetUsersUpdatedBetweenAsync(DateTime start, DateTime end)
    {
        var list = db.Users
            .Where(u => u.UpdateDate >= start && u.UpdateDate <= end)
            .Select(u => ToDto(u)!)
            .ToList();

        return Task.FromResult<ICollection<UserDto>>(list);
    }
    
    public Task<bool> UpdateUserAsync(UserDto user)
    {
        var existing = db.Users.FirstOrDefault(u => u.Id == user.Id);
        if (existing is null)
            return Task.FromResult(false);
        
        existing.Login = user.Login;
        existing.Password = user.Password;
        existing.Age = user.Age;
        existing.UpdateDate = DateTime.UtcNow;

        return Task.FromResult(true);
    }

    public Task SaveUserAsync(UserDto userDto)
    {
        var exists = db.Users.Any(u => u.Id == userDto.Id);
        if (!exists)
        {
            var entity = ToEntity(userDto);
            db.Users.Add(entity);
        }
        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(UserDto userDto)
    {
        var remaining = db.Users.Where(u => u.Id != userDto.Id).ToList();
        db.Users = new ConcurrentBag<UserEntity>(remaining);
        return Task.CompletedTask;
    }
}
