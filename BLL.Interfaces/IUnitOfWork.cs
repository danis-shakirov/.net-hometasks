using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Interfaces;

public interface IUnitOfWork
{
    DbSet<User> Users { get; }
    DbSet<Post> Posts { get; }
    DbSet<Comment> Comments { get; }
    Task<int> SaveChangesAsync();
}