using System.Collections.ObjectModel;

namespace Hive.Webhooks.Settings
{
    public class WebhookSettings
    {
        public ReadOnlyCollection<string> DiscordURLs { get; set; } = null!;
    }
}
