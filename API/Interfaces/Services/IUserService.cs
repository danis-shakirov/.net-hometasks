using API.DTOs;

namespace API.Interfaces.Services;

public interface IUserService
{
    Task<UserDto?> GetUserAsync(Guid id);
    Task<ICollection<UserDto>> GetUsersAsync();
    Task<ICollection<UserDto>> GetUsersCreatedByDateAsync(DateTime creationDate, bool laterThanDate);
    Task<ICollection<UserDto>> GetUsersUpdatedByDateAsync(DateTime updateDate, bool laterThanDate);
    Task<ICollection<UserDto>> GetUsersCreatedBetweenAsync(DateTime start, DateTime end);
    Task<ICollection<UserDto>> GetUsersUpdatedBetweenAsync(DateTime start, DateTime end);
    Task<bool> UpdateUserAsync(UserDto user);
}