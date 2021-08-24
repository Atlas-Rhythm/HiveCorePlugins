using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hive.Webhooks.Discord
{
    public class DiscordWebhook
    {
        public string? Content { get; }
#pragma warning disable CA1002 // Do not expose generic lists
        public List<DiscordEmbed>? Embeds { get; }
#pragma warning restore CA1002 // Do not expose generic lists

        [JsonPropertyName("avatar_url")]
        public Uri? AvatarURL { get; set; }
        public string? Username { get; set; }

        public DiscordWebhook(string content, string? username = null, Uri? avatarURL = null)
        {
            Content = content;
            Username = username;
            AvatarURL = avatarURL;
        }

#pragma warning disable CA1002 // Do not expose generic lists
        public DiscordWebhook(List<DiscordEmbed> embeds, string? username = null, Uri? avatarURL = null)
#pragma warning restore CA1002 // Do not expose generic lists
        {
            Embeds = embeds;
            Username = username;
            AvatarURL = avatarURL;
        }

        public DiscordWebhook(DiscordEmbed embed, string? username = null, Uri? avatarURL = null)
        {
            Embeds = new List<DiscordEmbed>();

            Embeds.Add(embed);
            Username = username;
            AvatarURL = avatarURL;
        }
    }
}
