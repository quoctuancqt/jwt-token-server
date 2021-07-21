using Microsoft.Extensions.Configuration;

namespace JwtTokenServer
{
    public static class ConfigExtensions
    {
        public static TConfig Bind<TConfig>(this IConfiguration configuration, string key = null) where TConfig : new()
        {
            var configs = new TConfig();

            configuration.Bind(key ?? typeof(TConfig).Name, configs);

            return configs;
        }
    }
}
