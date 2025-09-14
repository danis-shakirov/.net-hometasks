namespace API;

public record User(int Id, string Login, string Password, int Age);
public record LoginRequest(string Login, string Password);
public record RegisterRequest(string Login, string Password, int Age);
public record EditUserRequest(string? Password, int? Age);