using Hive.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.Webhooks
{
    [PluginStartup]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddHttpClient();
            _ = services.AddScoped<WebhookEmitter>();
        }
    }
}
