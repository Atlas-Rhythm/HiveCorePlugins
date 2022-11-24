using Hive.Permissions;
using Hive.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Hive.FileSystemRuleProvider
{
    [PluginStartup]
    public class Startup
    {
        private const string SplitToken = ".";
        private const string PathConfigurationKey = "RulePath";
        private const string PathDefaultValue = "Rules";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;

        public void ConfigureServices(IServiceCollection services)
        {
            var path = Configuration.GetValue(PathConfigurationKey, PathDefaultValue);

            _ = services.AddTransient<IRuleProvider>(sp =>
                    new FileSystemRuleProvider(sp.GetRequiredService<ILogger>(), SplitToken, path));
        }
    }
}
