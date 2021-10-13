using System;
using Hive.Models;
using Serilog;

namespace Hive.Webhooks.Discord
{
    internal class DiscordHiveWebhook : IHiveWebhook
    {
        private readonly ILogger _logger;

        public DiscordHiveWebhook(ILogger logger) => _logger = logger;

        public string ID => nameof(Discord);

        public object? ChannelCreated(Channel channel)
        {
            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            DiscordEmbed embed = new() { Title = "Channel Created" };
            embed.Fields.Add(new("Name", channel.Name));

            DiscordWebhook webhook = new(embed);
            return webhook;
        }

        public object? GameVersionCreated(GameVersion gameVersion)
        {
            if (gameVersion is null)
                throw new ArgumentNullException(nameof(gameVersion));

            DiscordEmbed embed = new() { Title = "Game Version Created" };
            embed.Fields.Add(new("Version", gameVersion.Name));

            DiscordWebhook webhook = new(embed);
            return webhook;
        }
    }
}
