using Hive.Models;

namespace Hive.Webhooks
{
    public class DefaultHiveWebhook : IHiveWebhook
    {
        public string ID => "Default";

        public object? ModMoved(Mod mod) => mod;

        public object? ModCreated(Mod mod) => mod;

        public object? ChannelCreated(Channel channel) => channel;

        public object? GameVersionCreated(GameVersion gameVersion) => gameVersion;
    }
}
