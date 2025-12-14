using Domain.Enums;

namespace Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public int Age { get; set; }
    public Role Role { get; set; }
    
    public DateTime CreationDate { get; set; }
    public DateTime UpdateDate { get; set; }
}