using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hive.Webhooks.Settings
{
    public class WebhookSettings
    {
        public ReadOnlyCollection<string> Discord { get; set; } = null!;

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
