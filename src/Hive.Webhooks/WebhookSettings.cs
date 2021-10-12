using System.Collections.Generic;

namespace Hive.Webhooks
{
    internal class WebhookSettings
    {
        public Dictionary<string, string[]> EmitTo { get; set; } = new();
    }
}
