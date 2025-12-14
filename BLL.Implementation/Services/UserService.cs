using BLL.Interfaces;
using BLL.Interfaces.Services;
using Domain.Dtos;
using Domain.Models;
using Domain.WebExceptions;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementation.Services;

public class UserService(IUnitOfWork uow, IMapper mapper) : IUserService
{
    public async Task<UserDto?> GetUserAsync(Guid id) => (await uow.Users.FindAsync(id)).Adapt<UserDto>();

    public async Task<ICollection<UserDto>> GetUsersAsync() =>
        await uow.Users.AsNoTracking().ProjectToType<UserDto>().ToListAsync();

    public async Task<ICollection<UserDto>> GetUsersCreatedByDateAsync(DateTime creationDate, bool laterThanDate)
    {
        var query = uow.Users.AsNoTracking().AsQueryable();

        return laterThanDate
            ? await query.Where(u => u.CreationDate <= creationDate).ProjectToType<UserDto>().ToListAsync()
            : await query.Where(u => u.CreationDate >= creationDate).ProjectToType<UserDto>().ToListAsync();
    }

    public async Task<ICollection<UserDto>> GetUsersUpdatedByDateAsync(DateTime updateDate, bool laterThanDate)
    {
        var query = uow.Users.AsNoTracking().AsQueryable();

        return laterThanDate
            ? await query.Where(u => u.UpdateDate <= updateDate).ProjectToType<UserDto>().ToListAsync()
            : await query.Where(u => u.UpdateDate >= updateDate).ProjectToType<UserDto>().ToListAsync();
    }

    public async Task<ICollection<UserDto>> GetUsersCreatedBetweenAsync(DateTime start, DateTime end)
    {
        NormalizeRange(ref start, ref end);

        return await uow.Users
            .AsNoTracking()
            .Where(u => u.CreationDate >= start && u.CreationDate <= end)
            .ProjectToType<UserDto>()
            .ToListAsync();
    }

    public async Task<ICollection<UserDto>> GetUsersUpdatedBetweenAsync(DateTime start, DateTime end)
    {
        NormalizeRange(ref start, ref end);

        return await uow.Users
            .AsNoTracking()
            .Where(u => u.UpdateDate >= start && u.UpdateDate <= end)
            .ProjectToType<UserDto>()
            .ToListAsync();
    }

    private static void NormalizeRange(ref DateTime start, ref DateTime end)
    {
        if (start > end)
            (start, end) = (end, start);
    }

    public async Task UpdateUserAsync(User user)
    {
        var existingUser = await uow.Users.FindAsync(user.Id);
        if (existingUser is null)
        {
            throw new NotFoundException("User not found");
        }

        user.Adapt(existingUser);

        await uow.SaveChangesAsync();
    }
}