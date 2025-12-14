using BLL.Interfaces;
using Domain.Models;

namespace API.GraphQL;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers([Service] IUnitOfWork uow) => uow.Users;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Post> GetPosts([Service] IUnitOfWork uow) => uow.Posts;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Comment> GetComments([Service] IUnitOfWork uow) => uow.Comments;
}