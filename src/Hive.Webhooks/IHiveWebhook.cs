using Hive.Models;

namespace Hive.Webhooks;

public interface IHiveWebhook
{
    public string Id { get; }

    public object? ModMoved(Mod mod) => null;

    public object? ModCreated(Mod mod) => null;

    public object? ChannelCreated(Channel channel) => null;

    public object? GameVersionCreated(GameVersion gameVersion) => null;
}
