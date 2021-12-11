﻿using DryIoc;
using Hive.Plugins;
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

        public void ConfigureContainer(IContainer container)
        {
            // TODO: Register your services in the IContainer
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
