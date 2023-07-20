using System;
using System.Text.Json.Serialization;

namespace Hive.Webhooks.Discord;

internal class DiscordAuthor
{
    [JsonPropertyName("proxy_icon_url")]
    public Uri? ProxyIconUrl { get; set; }

    [JsonPropertyName("icon_url")]
    public Uri? IconUrl { get; set; }

    public string? Name { get; set; }

    public Uri? Url { get; set; }
}