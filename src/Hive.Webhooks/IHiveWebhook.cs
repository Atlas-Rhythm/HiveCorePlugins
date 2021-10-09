using Hive.Models;

namespace Hive.Webhooks
{
    /// <summary>
    /// An interface for formatting objects into a desired format for a webhook.
    /// </summary>
    public interface IHiveWebhook
    {
        object? ModMoved(Mod mod) => null;

        object? ModCreated(Mod mod) => null;

        object? ChannelCreated(Channel channel) => null;

        object? GameVersionCreated(GameVersion gameVersion) => null;
    }
}
