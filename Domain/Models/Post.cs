namespace Domain.Models;

public class Post
{
    public long Id { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public List<Comment> Comments { get; set; } = [];
}