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
using Hive.Webhooks.Settings;
using Serilog;

namespace Hive.Webhooks
{
    public class HookEmitter : IChannelsControllerPlugin, IGameVersionsPlugin
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly WebhookSettings _webhookSettings;
        private readonly IEnumerable<IHookConverter> _hookConverters;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public HookEmitter([DisallowNull] ILogger logger, HttpClient httpClient, WebhookSettings webhookSettings, IEnumerable<IHookConverter> hookConverters)
        {
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            _httpClient = httpClient;
            _hookConverters = hookConverters;
            _webhookSettings = webhookSettings;
            _logger = logger.ForContext<HookEmitter>();
        }

        public void NewChannelCreated(Channel newChannel)
        {
            if (newChannel is null)
                throw new ArgumentNullException(nameof(newChannel));

            _logger.Debug("Generating hooks for {Channel}", newChannel.Name);
            foreach (var converter in _hookConverters)
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
            foreach (var converter in _hookConverters)
            {
                var hookObject = converter.GameVersionCreated(gameVersion);
                EmitHook(converter, hookObject);
            }
        }

        private void EmitHook(IHookConverter hookConverter, object? hook)
        {
            if (hook is null)
                return;

            _ = Task.Run(() => Emit(hookConverter, hook));
        }

        private async Task Emit(IHookConverter hookConverter, object? hook)
        {
            if (_webhookSettings.TryGetHookURLs(hookConverter.ID, out var urls))
            {
                foreach (var url in urls!)
                {
                    var bodyContent = JsonSerializer.Serialize(hook, _jsonSerializerOptions);
                    var body = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(url),
                        Content = new StringContent(bodyContent, Encoding.UTF8, "application/json"),
                    };
                    var response = await _httpClient.SendAsync(body).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.Information("Successfully executed a webhook to: {URL}", url);
                    }
                    else
                    {
                        _logger.Warning("Unable to successfully execute the webhook to {URL}: {ErrorCode}", url, response.StatusCode);
                    }
                }
            }
        }
    }
}
