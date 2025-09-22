using System.Security.Cryptography;
using API.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace API.Services;

public class SessionService(
    DbContextMock db,
    IHttpContextAccessor httpContextAccessor)
    : ISessionService
{
    private const string CookieName = "auth";

    private readonly IMemoryCache _sessionCache = db.SessionCache;

    public void CreateSession(Guid userId)
    {
        var context = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext недоступен");
        
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        
        var ttl = TimeSpan.FromHours(1);

        _sessionCache.Set(token, userId, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });
        
        var aspNetCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.Add(TimeSpan.FromHours(1)),
            IsEssential = true
        };

        context.Response.Cookies.Append(CookieName, token, aspNetCookieOptions);
    }

    public void DestroySession()
    {
        var context = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext недоступен");

        if (context.Request.Cookies.TryGetValue(CookieName, out var token) && !string.IsNullOrWhiteSpace(token))
        {
            _sessionCache.Remove(token);
        }
        
        context.Response.Cookies.Append(CookieName, string.Empty, new CookieOptions
        {
            Expires = DateTimeOffset.UnixEpoch,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            IsEssential = true
        });
    }
}
