using API.Filters;
using BLL.Implementation.Services;
using BLL.Interfaces.Services;
using FluentValidation;
using Mapster;
using MapsterMapper;

namespace API;

public static class ServiceCollectionExtensions
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddScoped(typeof(ValidationFilter<>));
    }

    public static void AddMapper(this IServiceCollection services, IConfiguration configuration)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(System.Reflection.Assembly.GetExecutingAssembly());

        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}