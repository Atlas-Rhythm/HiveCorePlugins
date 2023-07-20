using Hive.Controllers;
using Hive.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.AdditionalUserDataExposer
{
    [PluginStartup]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;

        public void ConfigureServices(IServiceCollection services)
        {
            var registration = services.AddSingleton<IUserPlugin, AdditionalUserDataExposerPlugin>();
            _ = registration.AddOptions<AdditionalUserDataExposerOptions>().Bind(Configuration);
        }
    }
}
