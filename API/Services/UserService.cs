using API.DTOs;
using API.Interfaces.Repositories;
using API.Interfaces.Services;

namespace API.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserDto?> GetUserAsync(Guid id) => await userRepository.GetUserByIdAsync(id);

    public async Task<ICollection<UserDto>> GetUsersAsync() => await userRepository.GetUsersAsync();

    public async Task<ICollection<UserDto>> GetUsersCreatedByDateAsync(DateTime creationDate, bool laterThanDate) =>
        await userRepository.GetUsersCreatedByDateAsync(creationDate, laterThanDate);

    public async Task<ICollection<UserDto>> GetUsersUpdatedByDateAsync(DateTime updateDate, bool laterThanDate) =>
        await userRepository.GetUsersUpdatedByDateAsync(updateDate, laterThanDate);
    
    public async Task<ICollection<UserDto>> GetUsersCreatedBetweenAsync(DateTime start, DateTime end)
    {
        NormalizeRange(ref start, ref end);
        return await userRepository.GetUsersCreatedBetweenAsync(start, end);
    }

    public async Task<ICollection<UserDto>> GetUsersUpdatedBetweenAsync(DateTime start, DateTime end)
    {
        NormalizeRange(ref start, ref end);
        return await userRepository.GetUsersUpdatedBetweenAsync(start, end);
    }

    private static void NormalizeRange(ref DateTime start, ref DateTime end)
    {
        if (start > end)
            (start, end) = (end, start);
    }

    public async Task<bool> UpdateUserAsync(UserDto user) => await userRepository.UpdateUserAsync(user);
}