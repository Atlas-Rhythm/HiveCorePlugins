using System;
using Hive.Models;

namespace Hive.Webhooks.Discord
{
    public class DiscordHookConverter : IHookConverter
    {
        public string ID => nameof(Discord);

        public object? ChannelCreated(Channel channel)
        {
            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            DiscordEmbed embed = new() { Title = "New Channel Created" };
            embed.Fields.Add(new("Name", channel.Name));

            DiscordWebhook webhook = new(embed);
            return webhook;
        }

        public object? GameVersionCreated(GameVersion gameVersion)
        {
            if (gameVersion is null)
                throw new ArgumentNullException(nameof(gameVersion));

            DiscordEmbed embed = new() { Title = "New Game Version Created" };
            embed.Fields.Add(new("Version", gameVersion.Name));

            DiscordWebhook webhook = new(embed);
            return webhook;
        }
    }
}
