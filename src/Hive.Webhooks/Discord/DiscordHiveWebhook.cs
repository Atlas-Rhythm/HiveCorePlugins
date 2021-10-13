using Hive.Models;
using Serilog;

namespace Hive.Webhooks.Discord
{
    internal class DiscordHiveWebhook : IHiveWebhook
    {
        private readonly ILogger _logger;

        public DiscordHiveWebhook(ILogger logger)
        {
            _logger = logger;
        }

        public string ID => nameof(Discord);

        public object? ChannelCreated(Channel channel)
        {
            _logger.Warning("DISCORD HIVE WEBHOOK CHANNEL CREATED {ChannelName}", channel.Name);
            return channel;
        }
    }
}
