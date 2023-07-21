using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Hive.Controllers;
using Hive.Models;
using Hive.Plugins.Aggregates;
using Hive.Services.Common;
using Serilog;

namespace Hive.Webhooks;

public class WebhookEmitter : IChannelsControllerPlugin, IGameVersionsPlugin, IModsPlugin, IUploadPlugin
{
    private readonly ILogger _logger;
    private readonly WebhookSettings _webhookSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<IHiveWebhook> _hiveWebhooks;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public WebhookEmitter(ILogger logger, WebhookSettings webhookSettings, IHttpClientFactory httpClientFactory, IEnumerable<IHiveWebhook> hiveWebhooks)
    {
        if (logger is null)
            throw new ArgumentNullException(nameof(logger));

        _hiveWebhooks = hiveWebhooks;
        _webhookSettings = webhookSettings;
        _httpClientFactory = httpClientFactory;
        _logger = logger.ForContext<WebhookEmitter>();
    }

    public void NewChannelCreated(Channel newChannel)
    {
        if (newChannel is null)
            throw new ArgumentNullException(nameof(newChannel));

        _logger.Debug("Generating hooks for {Channel} (created)", newChannel.Name);
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

        _logger.Debug("Generating hooks for {GameVersion} (created)", gameVersion.Name);
        foreach (var webhook in _hiveWebhooks)
        {
            var hookObject = webhook.GameVersionCreated(gameVersion);
            EmitHook(webhook, hookObject);
        }
    }

    public void ModifyAfterModMove(in Mod input)
    {
        if (input is null)
            throw new ArgumentNullException(nameof(input));

        _logger.Debug("Generating hooks for {Mod} (moved)", input.ReadableID);
        foreach (var webhook in _hiveWebhooks)
        {
            var hookObject = webhook.ModMoved(input);
            EmitHook(webhook, hookObject);
        }
    }

    [return: StopIfReturns(false)]
    public bool ValidateAndPopulateKnownMetadata(Mod mod, Stream data, [ReturnLast] out object? validationFailureInfo)
    {
        validationFailureInfo = null;
        return true;
    }

    [return: StopIfReturns(false)]
    public bool ValidateAndFixUploadedData(Mod mod, ArbitraryAdditionalData originalAdditionalData, [ReturnLast] out object? validationFailureInfo)
    {
        validationFailureInfo = null;
        return true;
    }

    public void UploadFinished(Mod modData)
    {
        if (modData is null)
            throw new ArgumentNullException(nameof(modData));

        _logger.Debug("Generating hooks for {Mod} (created)", modData.ReadableID);
        foreach (var webhook in _hiveWebhooks)
        {
            var hookObject = webhook.ModCreated(modData);
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
        if (_webhookSettings.EmitTo.TryGetValue(hookConverter.Id, out var urls))
        {
            var serializerOptions = JsonSerializerOptions;
            foreach (var url in urls)
            {
                var body = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Content = new StringContent(JsonSerializer.Serialize(hook, options: serializerOptions), Encoding.UTF8, "application/json"),
                };

                var httpClient = _httpClientFactory.CreateClient("Hive.Webhooks");
                var response = await httpClient.SendAsync(body).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Warning("Unable to successfully execute the webhook to {Url}: {ErrorCode}", url, response.StatusCode);
                    return;
                }
                _logger.Information("Successfully executed a webhook to: {Url}", url);
            }
        }
    }
}
