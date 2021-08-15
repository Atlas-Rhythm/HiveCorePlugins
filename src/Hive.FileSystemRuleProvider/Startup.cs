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
        private const string SubfolderConfigurationKey = "RuleSubfolder";
        private const string SubfolderDefaultValue = "Rules";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;

        public void ConfigureServices(IServiceCollection services)
        {
            var subfolder = Configuration.GetValue(SubfolderConfigurationKey, SubfolderDefaultValue);

            _ = services.AddTransient<IRuleProvider>(sp =>
                    new FileSystemRuleProvider(sp.GetRequiredService<ILogger>(), SplitToken, subfolder));
        }
    }
}
