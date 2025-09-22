using API.DTOs;

namespace API.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByLogin(string login);
    Task<UserDto?> GetUserByCredentialsAsync(string login, string password);
    
    Task<ICollection<UserDto>> GetUsersAsync();
    Task<ICollection<UserDto>> GetUsersCreatedByDateAsync(DateTime creationDate, bool laterThanDate);
    Task<ICollection<UserDto>> GetUsersUpdatedByDateAsync(DateTime updateDate, bool laterThanDate);
    
    Task<ICollection<UserDto>> GetUsersCreatedBetweenAsync(DateTime start, DateTime end);
    Task<ICollection<UserDto>> GetUsersUpdatedBetweenAsync(DateTime start, DateTime end);
        
    Task<bool> UpdateUserAsync(UserDto user);
    
    Task SaveUserAsync(UserDto userDto);
    Task DeleteUserAsync(UserDto userDto);
}