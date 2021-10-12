using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Hive.Models;
using Hive.Services.Common;
using Serilog;

namespace Hive.Webhooks
{
    internal class WebhookEmitter : IChannelsControllerPlugin, IGameVersionsPlugin
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IEnumerable<IHiveWebhook> _hiveWebhooks;

        public WebhookEmitter([DisallowNull] ILogger logger, HttpClient httpClient, IEnumerable<IHiveWebhook> hiveWebhooks)
        {
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            _httpClient = httpClient;
            _hiveWebhooks = hiveWebhooks;
            _logger = logger.ForContext<WebhookEmitter>();
        }

        public void NewChannelCreated(Channel newChannel)
        {
            if (newChannel is null)
                throw new ArgumentNullException(nameof(newChannel));

            _logger.Debug("Generating hooks for {Channel}", newChannel.Name);
            foreach (var converter in _hiveWebhooks)
            {
                var hookObject = converter.ChannelCreated(newChannel);
                EmitHook(converter, hookObject);
            }
        }

        public void NewGameVersionCreated(GameVersion gameVersion)
        {
            if (gameVersion is null)
                throw new ArgumentNullException(nameof(gameVersion));

            _logger.Debug("Generating hooks for {GameVersion}", gameVersion.Name);
            foreach (var webhook in _hiveWebhooks)
            {
                var hookObject = webhook.GameVersionCreated(gameVersion);
                EmitHook(webhook, hookObject);
            }
        }

        private void EmitHook(IHiveWebhook hiveWebhook, object? body)
        {
            if (body is null)
                return;

            _ = Task.Run(() => Emit(hiveWebhook, body));
        }

        private Task Emit(IHiveWebhook hookConverter, object? hook) => throw new NotImplementedException();
    }
}
