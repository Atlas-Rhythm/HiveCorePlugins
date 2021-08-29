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
            _ = services.Configure<FileSystemCdnProvider>(Configuration);
            _ = services.AddTransient<ICdnProvider, FileSystemCdnProvider>();
        }
    }
}
