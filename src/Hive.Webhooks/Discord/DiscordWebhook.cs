using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hive.Webhooks.Discord
{
    internal class DiscordWebHook
    {
        public string? Content { get; }
        public List<DiscordEmbed>? Embeds { get; }

        [JsonPropertyName("avatar_url")]
        public Uri? AvatarURL { get; set; }
        public string? Username { get; set; }

        public DiscordWebHook(string content, string? username = null, Uri? avatarURL = null)
        {
            Content = content;
            Username = username;
            AvatarURL = avatarURL;
        }

        public DiscordWebHook(List<DiscordEmbed> embeds, string? username = null, Uri? avatarURL = null)
        {
            Embeds = embeds;
            Username = username;
            AvatarURL = avatarURL;
        }
    }
}
