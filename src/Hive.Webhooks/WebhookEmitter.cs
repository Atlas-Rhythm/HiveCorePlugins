using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private readonly WebhookSettings _webhookSettings;
        private readonly IEnumerable<IHiveWebhook> _hiveWebhooks;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public WebhookEmitter([DisallowNull] ILogger logger, HttpClient httpClient, WebhookSettings webhookSettings, IEnumerable<IHiveWebhook> hiveWebhooks)
        {
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            _httpClient = httpClient;
            _hiveWebhooks = hiveWebhooks;
            _webhookSettings = webhookSettings;
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

        private async Task Emit(IHiveWebhook hookConverter, object? hook)
        {
            if (_webhookSettings.EmitTo.TryGetValue(hookConverter.ID, out var urls))
            {
                var serializerOptions = (hookConverter is IWebhookSerializeSettings customSerializer) ? customSerializer.Options : _jsonSerializerOptions;
                foreach (var url in urls)
                {
                    var body = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(url),
                        Content = new StringContent(JsonSerializer.Serialize(hook, options: serializerOptions), Encoding.UTF8, "application/json"),
                    };
                    var response = await _httpClient.SendAsync(body).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.Warning("Unable to successfully execute the webhook to {URL}: {ErrorCode}", url, response.StatusCode);
                        return;
                    }
                    _logger.Information("Successfully executed a webhook to: {URL}", url);
                }
            }
        }
    }
}
