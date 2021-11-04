using Hive.Plugins;
using Hive.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.FileSystemCdnProvider
{
    [PluginStartup]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddSingleton(Configuration);
            _ = services.AddSingleton<ICdnProvider, FileSystemCdnProvider>();
        }
    }
}
