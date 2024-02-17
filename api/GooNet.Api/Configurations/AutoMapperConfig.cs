using GooNet.Application.AutoMapper;

namespace GooNet.Configurations;

public static class AutoMapperConfig
{
    public static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddAutoMapper(typeof(DomainToDtoMappingProfile));
    }
}