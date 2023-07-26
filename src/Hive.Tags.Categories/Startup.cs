using DryIoc;
using Hive.Plugins;
using Hive.Tags.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hive.Tags.Categories;

[PluginStartup]
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration config) => _configuration = config;

    public void ConfigureServices(IServiceCollection services)
    {
        _ = services.Configure<CategoryOptions>(_configuration);
        _ = services.AddSingleton(sp => sp.GetRequiredService<IOptions<CategoryOptions>>().Value);
    }

    public static void ConfigureContainer(Container container) => container.Register<ITagPlugin, CategoryTagPlugin>();
}
