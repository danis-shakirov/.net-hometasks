namespace API.DTOs;

public record UserDto(Guid Id, string Login, string Password, int Age);