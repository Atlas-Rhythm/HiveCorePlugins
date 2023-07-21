using System;
using System.Text.Json.Serialization;

namespace Hive.Webhooks.Discord;

internal class DiscordMedia
{
    [JsonPropertyName("proxy_url")]
    public Uri? ProxyUrl { get; set; }

    public Uri? Url { get; set; }

    public int? Height { get; set; }

    public int? Width { get; set; }
}