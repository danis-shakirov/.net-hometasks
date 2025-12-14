namespace BLL.Implementation.Auth;

public class JwtOptions
{
    public required string SecretKey { get; set; }
    public int ExpiresInMinutes { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}