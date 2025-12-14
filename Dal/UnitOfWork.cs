using BLL.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public DbSet<User> Users => dbContext.Users;
    public DbSet<Post> Posts => dbContext.Posts;
    public DbSet<Comment> Comments => dbContext.Comments;

    public async Task<int> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync();
    }
}