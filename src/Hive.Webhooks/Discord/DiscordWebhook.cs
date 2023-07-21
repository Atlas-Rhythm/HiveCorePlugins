using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hive.Webhooks.Discord;

internal class DiscordWebhook
{
    public string? Content { get; }
    public List<DiscordEmbed>? Embeds { get; }

    [JsonPropertyName("avatar_url")]
    public Uri? AvatarUrl { get; set; }
    public string? Username { get; set; }

    public DiscordWebhook(string content, string? username = null, Uri? avatarUrl = null)
    {
        Content = content;
        Username = username;
        AvatarUrl = avatarUrl;
    }

    public DiscordWebhook(List<DiscordEmbed> embeds, string? username = null, Uri? avatarUrl = null)
    {
        Embeds = embeds;
        Username = username;
        AvatarUrl = avatarUrl;
    }

    public DiscordWebhook(DiscordEmbed embed, string? username = null, Uri? avatarUrl = null)
    {
        Embeds = new();
        Embeds.Add(embed);
        Username = username;
        AvatarUrl = avatarUrl;
    }
}
