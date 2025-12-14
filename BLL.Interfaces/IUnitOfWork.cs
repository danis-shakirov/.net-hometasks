using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Interfaces;

public interface IUnitOfWork
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync();
}