using System;
using System.Collections.Generic;

namespace Hive.Webhooks.Discord;

/// <see cref="https://discord.com/developers/docs/resources/channel#embed-object" />
internal class DiscordEmbed
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public Uri? Url { get; set; }

    public DateTime? Timestamp { get; set; }

    public int? Color { get; set; }

    public DiscordFooter? Footer { get; set; }

    public DiscordMedia? Image { get; set; }

    public DiscordMedia? Thumbnail { get; set; }

    public DiscordMedia? Video { get; set; }

    public DiscordProvider? Provider { get; set; }

    public List<DiscordField> Fields { get; set; } = new();
}