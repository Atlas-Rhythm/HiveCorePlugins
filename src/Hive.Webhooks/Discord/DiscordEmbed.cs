using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hive.Webhooks.Discord
{
    /// <see cref="https://discord.com/developers/docs/resources/channel#embed-object"/>
    public class DiscordEmbed
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Uri? URL { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? Color { get; set; }
        public DiscordFooter? Footer { get; set; }
        public DiscordMedia? Image { get; set; }
        public DiscordMedia? Thumbnail { get; set; }
        public DiscordMedia? Video { get; set; }
        public DiscordProvider? Provider { get; set; }
#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable CA2227 // Collection properties should be read only
        public List<DiscordField>? Fields { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore CA1002 // Do not expose generic lists
    }

    public class DiscordFooter
    {
        public string Text { get; set; } = null!;

        [JsonPropertyName("icon_url")]
        public Uri? IconURL { get; set; }

        [JsonPropertyName("proxy_icon_url")]
        public Uri? ProxyIconURL { get; set; }
    }

    public class DiscordField
    {
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
        public bool? Inline { get; set; }
    }

    public class DiscordMedia
    {
        [JsonPropertyName("proxy_url")]
        public Uri? ProxyURL { get; set; }

        public Uri? URL { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
    }

    public class DiscordProvider
    {
        public string? Name { get; set; }
        public Uri? URL { get; set; }
    }

    public class DiscordAuthor
    {
        [JsonPropertyName("proxy_icon_url")]
        public Uri? ProxyIconURL { get; set; }

        [JsonPropertyName("icon_url")]
        public Uri? IconURL { get; set; }

        public string? Name { get; set; }
        public Uri? URL { get; set; }
    }
}
