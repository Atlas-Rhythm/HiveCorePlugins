using System.Collections.Generic;
using Hive.Webhooks.Discord;

namespace Hive.Webhooks;

public class WebhookSettings
{
    public Dictionary<string, string[]> EmitTo { get; set; } = new();

    public DiscordHookSettings Discord { get; set; } = new();
}
