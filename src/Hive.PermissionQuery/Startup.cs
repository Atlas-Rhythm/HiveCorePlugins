﻿using Hive.Plugins;
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


        public void ConfigureServices(IServiceCollection services)
            => _ = services.AddOptions<PermissionQueryOptions>()
                .Bind(Configuration);
    }
}
