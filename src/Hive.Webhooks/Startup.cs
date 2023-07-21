using DryIoc;
using Hive.Plugins;
using Hive.Webhooks.Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hive.Webhooks;

[PluginStartup]
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration config) => _configuration = config;

    public void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddHttpClient("Hive.Webhooks");
        _ = services.Configure<WebhookSettings>(_configuration);
        _ = services.AddSingleton(sp => sp.GetRequiredService<IOptions<WebhookSettings>>().Value);
        _ = services.AddSingleton(sp => sp.GetRequiredService<IOptions<WebhookSettings>>().Value.Discord);
    }

    public static void ConfigureContainer(Container container)
    {
        container.RegisterMany<WebhookEmitter>(Reuse.Singleton);
        container.Register<IHiveWebhook, DefaultHiveWebhook>(Reuse.Singleton);
        container.Register<IHiveWebhook, DiscordHiveWebhook>(Reuse.Singleton);
    }
}
