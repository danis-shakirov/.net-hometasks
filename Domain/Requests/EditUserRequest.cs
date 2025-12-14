namespace Domain.Requests;

public record EditUserRequest(string? Password, int? Age);