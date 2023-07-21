using System;
using Hive.Models;
using Serilog;

namespace Hive.Webhooks.Discord;

public class DiscordHiveWebhook : IHiveWebhook
{
    public string Id => nameof(Discord);

    private readonly ILogger _logger;
    private readonly DiscordHookSettings _discordHookSettings;

    public DiscordHiveWebhook(ILogger logger, DiscordHookSettings discordHookSettings)
    {
        _logger = logger;
        _discordHookSettings = discordHookSettings;
    }

    public object ChannelCreated(Channel channel)
    {
        if (channel is null)
            throw new ArgumentNullException(nameof(channel));

        DiscordEmbed embed = new()
        {
            Title = "Channel Created",
            Color = _discordHookSettings.CreatedColorValue
        };
        embed.Fields.Add(new DiscordField("Name", channel.Name));

        DiscordWebhook webhook = new(embed);
        return webhook;
    }

    public object GameVersionCreated(GameVersion gameVersion)
    {
        if (gameVersion is null)
            throw new ArgumentNullException(nameof(gameVersion));

        DiscordEmbed embed = new()
        {
            Title = "Game Version Created",
            Color = _discordHookSettings.CreatedColorValue
        };
        embed.Fields.Add(new DiscordField("Version", gameVersion.Name));

        DiscordWebhook webhook = new(embed);
        return webhook;
    }

    public object ModMoved(Mod mod)
    {
        if (mod is null)
            throw new ArgumentNullException(nameof(mod));

        DiscordEmbed embed = new()
        {
            Title = $"{mod.ReadableID} {mod.Version} has been moved to {mod.Channel.Name}",
            Color = _discordHookSettings.NeutralColorValue
        };

        DiscordWebhook webhook = new(embed);
        return webhook;
    }

    public object ModCreated(Mod mod)
    {
        if (mod is null)
            throw new ArgumentNullException(nameof(mod));

        DiscordEmbed embed = new()
        {
            Title = $"{mod.Uploader.Username} uploaded {mod.ReadableID} {mod.Version}",
            Color = _discordHookSettings.NeutralColorValue
        };

        DiscordWebhook webhook = new(embed);
        return webhook;
    }
}
