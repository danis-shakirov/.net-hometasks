using System.Collections.Concurrent;
using API.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace API;

public class DbContextMock
{
    public ConcurrentBag<UserEntity> Users { get; set; } = [];

    public IMemoryCache SessionCache { get; } = new MemoryCache(new MemoryCacheOptions());
}