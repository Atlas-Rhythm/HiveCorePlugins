using Hive.Models;

namespace Hive.Webhooks
{
    /// <summary>
    /// An interface for formatting objects into a desired format for a webhook.
    /// </summary>
    public interface IHiveWebhook
    {
        public string ID { get; }

        public object? ModMoved(Mod mod) => null;

        public object? ModCreated(Mod mod) => null;

        public object? ChannelCreated(Channel channel) => null;

        public object? GameVersionCreated(GameVersion gameVersion) => null;
    }
}
