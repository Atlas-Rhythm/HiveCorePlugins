using Hive.Models;

namespace Hive.Webhooks
{
    public interface IHookConverter
    {
        string ID { get; }
        public object? ModCreated(Mod mod) => null;
        public object? ChannelCreated(Channel channel) => null;
        public object? GameVersionCreated(GameVersion gameVersion) => null;
    }
}
