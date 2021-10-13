using System.Text.Json;

namespace Hive.Webhooks
{
    /// <summary>
    /// Used for providing overriding of JSON serialization settings during webhook creation.
    /// </summary>
    public interface IWebhookSerializeSettings
    {
        public JsonSerializerOptions Options { get; }
    }
}
