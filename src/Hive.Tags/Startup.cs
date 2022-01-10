using DryIoc;
using Hive.Controllers;
using Hive.Graphing;
using Hive.Graphing.Types;
using Hive.Plugins;
using Hive.Tags.Extensions;
using Hive.Tags.Graphing;
using Hive.Tags.Plugins;
using Microsoft.Extensions.Configuration;

namespace Hive.Tags
{
    [PluginStartup]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;

        public static void ConfigureContainer(IContainer container)
        {
            container.RegisterCustomFunctions();

            // REVIEW: Is this the right Reuse?
            container.Register<IUploadPlugin, TagUploadPlugin>(Reuse.Scoped);

            container.Register<ICustomHiveGraph<ModType>, TagsGraphType>(Reuse.Singleton);
        }
    }
}
