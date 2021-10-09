using Hive.Plugins;
using Microsoft.Extensions.Configuration;

namespace Hive.Webhooks
{
    [PluginStartup]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;
    }
}
