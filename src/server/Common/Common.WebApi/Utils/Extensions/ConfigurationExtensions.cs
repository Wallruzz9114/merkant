using Microsoft.Extensions.Configuration;
using Mono.TextTemplating;

namespace Common.WebApi.Utils.Extensions;

public static class ConfigurationExtensions
{
    public static TSettings BindSettings<TSettings>(this IConfiguration configuration, string sectionKey) where TSettings : new()
    {
        TSettings settings = new();
        configuration.GetSection(sectionKey).Bind(settings);

        return settings;
    }
}