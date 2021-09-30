using System.Text.Json.Serialization;
using NodaTime;

namespace Hive.FileSystemCdnProvider
{
    /// <summary>
    /// Basic data structure for a CDN Entry in a metadata file.
    /// </summary>
    public record FileSystemCdnEntry
    {
        /// <summary>
        /// The real file name for this CDN object.
        /// </summary>
        public string ObjectName { get; init; }

        /// <summary>
        /// If not null, this is the <see cref="Instant"/> that the CDN Entry is considered expired,
        /// and will be marked for removal via <see cref="MarkedForCleanup"/>
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Instant? ExpiresAt { get; set; }

        /// <summary>
        /// If <see cref="true"/>, the Janitor will clear up the CDN Entry on its next sweep.
        /// </summary>
        public bool MarkedForCleanup { get; set; }

        public FileSystemCdnEntry(string name, Instant? expiresAt = null)
        {
            ObjectName = name;
            ExpiresAt = expiresAt;
        }
    }
}
