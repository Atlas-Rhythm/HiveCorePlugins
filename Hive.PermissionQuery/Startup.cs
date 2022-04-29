using DryIoc;
using Hive.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.PermissionQuery
{
    [PluginStartup]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;


        public static void ConfigureServices(IServiceCollection services)
            // REVIEW: Does Hive automatically add REST controllers in plugins?
            => _ = services.AddControllers();

        public static void ConfigureContainer(IContainer container)
        {
            // TODO: Configuration
        }
    }
}
