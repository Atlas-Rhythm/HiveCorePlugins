using DryIoc;
using Hive.Controllers;
using Hive.Graphing;
using Hive.Graphing.Types;
using Hive.Plugins;
using Hive.Tags.Extensions;
using Hive.Tags.Graphing;
using Hive.Tags.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

        public /*async Task*/ void PreConfigure/*Async*/()
        {
            // TODO: Perform any possibly-asynchronous setup that needs to happen before Configure.
            //   If this method needs to be async, it MUST be named PreConfigureAsync and return either
            // Task or ValueTask. Both the sync and async versions may take any services as parameters.
            // They are automatically injected, much like Configure.
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // TODO: Configure the application
        }
    }
}
