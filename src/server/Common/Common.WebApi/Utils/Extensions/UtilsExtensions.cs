using Common.WebApi.Utils.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Common.WebApi.Utils.Extensions;

public static class UtilsExtensions
{
    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        services.AddScoped<IFileUtils, FileUtils>();
        services.AddScoped<IValidateUtils, ValidateUtils>();

        return services;
    }
}