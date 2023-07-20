using System;
using System.Text.Json.Serialization;

namespace Hive.Webhooks.Discord;

internal class DiscordFooter
{
    public string Text { get; set; } = null!;

    [JsonPropertyName("icon_url")]
    public Uri? IconUrl { get; set; }

    [JsonPropertyName("proxy_icon_url")]
    public Uri? ProxyIconUrl { get; set; }
}