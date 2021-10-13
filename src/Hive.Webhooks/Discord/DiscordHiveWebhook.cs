using System;
using Hive.Models;
using Serilog;

namespace Hive.Webhooks.Discord
{
    internal class DiscordHiveWebhook : IHiveWebhook
    {
        public string ID => nameof(Discord);

        private readonly ILogger _logger;
        private readonly DiscordHookSettings _discordHookSettings;

        public DiscordHiveWebhook(ILogger logger, DiscordHookSettings discordHookSettings)
        {
            _logger = logger;
            _discordHookSettings = discordHookSettings;
        }

        public object? ChannelCreated(Channel channel)
        {
            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            DiscordEmbed embed = new() { Title = "Channel Created" };
            embed.Color = _discordHookSettings.CreatedColorValue;
            embed.Fields.Add(new("Name", channel.Name));

            DiscordWebhook webhook = new(embed);
            return webhook;
        }

        public object? GameVersionCreated(GameVersion gameVersion)
        {
            if (gameVersion is null)
                throw new ArgumentNullException(nameof(gameVersion));

            DiscordEmbed embed = new() { Title = "Game Version Created" };
            embed.Color = _discordHookSettings.NeutralColorValue;
            embed.Fields.Add(new("Version", gameVersion.Name));

            DiscordWebhook webhook = new(embed);
            return webhook;
        }
    }
}
