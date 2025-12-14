using Domain.Dtos;
using Domain.Models;
using Mapster;

namespace API.Configurations;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<User, UserDto>
            .NewConfig();

        TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
    }
}