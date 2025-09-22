namespace API.DTOs;

public record RegisterRequest(string Login, string Password, int Age);