namespace Domain.Requests;

public record RegisterRequest(string Login, string Password, int Age);