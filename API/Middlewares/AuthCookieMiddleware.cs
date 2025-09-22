namespace API.Middlewares;

public class AuthCookieMiddleware(RequestDelegate next)
{
    private const string CookieName = "auth";
    private const string UserIdItemKey = "UserId";

    public async Task InvokeAsync(HttpContext context, DbContextMock db)
    {
        if (!context.Items.ContainsKey(UserIdItemKey) &&
            context.Request.Cookies.TryGetValue(CookieName, out var token) &&
            !string.IsNullOrWhiteSpace(token) &&
            db.SessionCache.TryGetValue(token, out var userId))
        {
            context.Items[UserIdItemKey] = userId;
        }

        await next(context);
    }
}