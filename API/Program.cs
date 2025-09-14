using System.Security.Cryptography;
using API;
using Microsoft.Extensions.Caching.Memory;

List<User> users =
[
    new User(1, "Danis", "superPA55word", 20),
    new User(0, "admin", "admin", 999)
];
int ids = 2;
IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/users/{id:int}",
    (int id) =>
    {
        var user = users.Find(x => x.Id == id);

        return user is null
            ? Results.NotFound()
            : Results.Ok(new
            {
                user.Id,
                user.Login,
                user.Age
            });
    });

app.MapPost("/users/login", (LoginRequest dto, HttpResponse resp) =>
{
    var user = FindByCredentials(dto.Login, dto.Password);
    if (user is null)
        return Results.Unauthorized();

    CreateSession(resp, user.Id);

    return Results.Ok(new { expires_in = 3600 });
});

app.MapPost("/users/register", (RegisterRequest dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrWhiteSpace(dto.Password))
        return Results.BadRequest("login/password обязательны");

    if (dto.Age <= 0) return Results.BadRequest("age должен быть больше 0");

    var ok = Register(dto.Login, dto.Password, dto.Age);

    return ok
        ? Results.Created($"/users/{ids - 1}", new { Id = ids - 1, dto.Login, dto.Age })
        : Results.Conflict("Пользователь с таким логином уже существует");
});

app.MapPut("/users/{id:int}", (int id, EditUserRequest dto, HttpRequest req) =>
{
    if (!TryGetAuthorizedUserId(req, out var requesterId))
        return Results.Unauthorized();

    var requester = users.FirstOrDefault(u => u.Id == requesterId);
    if (requester is null)
        return Results.Unauthorized();

    if (users.All(u => u.Id != id))
        return Results.NotFound();

    var isAdmin = requester.Id == 0;
    if (!isAdmin && requesterId != id)
        return Results.Forbid();

    var ok = EditUser(id, dto.Password, dto.Age);

    return ok
        ? Results.Ok()
        : Results.Problem("Не удалось обновить пользователя");
});

app.MapDelete("/users/{id:int}", (int id, HttpRequest req) =>
{
    if (!TryGetAuthorizedUserId(req, out var requesterId))
        return Results.Unauthorized();

    var requester = users.FirstOrDefault(u => u.Id == requesterId);
    if (requester is null) return Results.Unauthorized();

    var isAdmin = string.Equals(requester.Login, "admin", StringComparison.OrdinalIgnoreCase);
    if (!isAdmin && requesterId != id)
        return Results.Forbid();

    var ok = DeleteUser(id);

    return ok
        ? Results.NoContent()
        : Results.NotFound();
});

app.MapPost("/users/logout", (HttpRequest req, HttpResponse resp) =>
{
    DestroySession(req, resp);

    return Results.Ok();
});

app.Run();
return;


CookieOptions AuthCookieOptions() => new()
{
    HttpOnly = true,
    SameSite = SameSiteMode.Strict,
    Expires = DateTimeOffset.UtcNow.Add(TimeSpan.FromHours(1)),
    IsEssential = true
};

void CreateSession(HttpResponse resp, int userId)
{
    var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    cache.Set(token, userId, new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    });

    resp.Cookies.Append("auth", token, AuthCookieOptions());
}

bool TryGetAuthorizedUserId(HttpRequest req, out int userId)
{
    userId = -1;
    if (!req.Cookies.TryGetValue("auth", out var token) || string.IsNullOrWhiteSpace(token))
        return false;

    var key = token;

    return cache.TryGetValue(key, out userId);
}

void DestroySession(HttpRequest request, HttpResponse response)
{
    if (request.Cookies.TryGetValue("auth", out var token))
        cache.Remove(token);

    response.Cookies.Append("auth", "", new CookieOptions
    {
        Expires = DateTimeOffset.UnixEpoch,
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        IsEssential = true
    });
}

User? FindByCredentials(string login, string password)
    => users.FirstOrDefault(x => x.Login == login && x.Password == password);

bool Register(string login, string password, int age)
{
    if (users.Any(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
        return false;

    var user = new User(ids++, login, password, age);
    users.Add(user);

    return true;
}

bool DeleteUser(int id)
{
    var idx = users.FindIndex(u => u.Id == id);
    if (idx < 0) return false;
    users.RemoveAt(idx);

    return true;
}

bool EditUser(int id, string? newPassword, int? newAge)
{
    var i = users.FindIndex(x => x.Id == id);
    if (i < 0) return false;

    var current = users[i];
    var updated = current with
    {
        Password = !string.IsNullOrEmpty(newPassword) ? newPassword : current.Password,
        Age = newAge ?? current.Age
    };
    users[i] = updated;

    return true;
}