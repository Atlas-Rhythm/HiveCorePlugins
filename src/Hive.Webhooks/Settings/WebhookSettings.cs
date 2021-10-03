using System.Collections.Generic;

namespace Hive.Webhooks.Settings
{
    public class WebhookSettings
    {
#pragma warning disable CA1002 // Do not expose generic lists
        public List<string> Discord { get; init; } = null!;
#pragma warning restore CA1002 

        public bool TryGetHookURLs(string id, out IEnumerable<string>? urls)
        {
            if (id == nameof(Discord))
            {
                urls = Discord;
                return true;
            }
            urls = null;
            return false;
        }
    }
}
