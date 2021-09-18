using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Hive.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NodaTime;
using Serilog;

namespace Hive.FileSystemCdnProvider
{
    // TODO: Test
    public class FileSystemCdnProvider : ICdnProvider
    {
        private const string SubfolderConfigurationKey = "CdnSubfolder";
        private const string SubfolderDefaultValue = "cdn";

        private const string MetadataFileConfigurationKey = "MetadataFileName";
        private const string MetadataFileDefaultValue = "metadata.json";

        private readonly ILogger logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string cdnSubfolder;
        private readonly string cdnPath;
        private readonly string metadataPath;
        private readonly FileSystemCdnMetadata cdnMetadata = new();

        public FileSystemCdnProvider(ILogger logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;

            cdnSubfolder = configuration.GetValue(SubfolderConfigurationKey, SubfolderDefaultValue);
            var metadata = configuration.GetValue(MetadataFileConfigurationKey, MetadataFileDefaultValue);

            cdnPath = Path.Combine(Directory.GetCurrentDirectory(), cdnSubfolder);
            metadataPath = Path.Combine(cdnPath, metadata);

            // Read our metadata file from disk and populate our dictionary
            // REVIEW: Is it dumb to store the metadata in the cdn subdirectory?
            if (File.Exists(metadataPath))
            {
                var metadataFromDisk = JsonSerializer.Deserialize<FileSystemCdnMetadata>(File.ReadAllText(metadataPath));

                if (metadataFromDisk != null)
                {
                    cdnMetadata = metadataFromDisk;
                }
            }

            _ = Directory.CreateDirectory(cdnPath);
        }

        public async Task<CdnObject> UploadObject(string name, Stream data, Instant? expireAt)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            logger.Information("Uploading {0} to File System CDN...", name);

            // REVIEW: The idea here is to prevent possible collisions by going off the hash of the stream. Is this stupid?
            var streamHash = data.GetHashCode().ToString(CultureInfo.InvariantCulture);

            // The UploadController already seeks to the beginning, but this is just in case another caller forgets to
            _ = data.Seek(0, SeekOrigin.Begin);

            // CDN path shall be <Hive>/<Subfolder>/<Stream Hash>/<File Name>
            var objectCdnPath = Path.Combine(cdnPath, streamHash, name);

            _ = Directory.CreateDirectory(objectCdnPath);

            // Lets construct a file stream and copy our data to that file.
            using var fileStream = File.Create(objectCdnPath);

            await data.CopyToAsync(fileStream).ConfigureAwait(false);

            // Construct CDN data and add it to our dictionary. Unique ID is the hash of the data stream
            var cdnObject = new CdnObject(streamHash);
            var cdnEntry = new FileSystemCdnEntry(cdnObject.UniqueId, expireAt);

            cdnMetadata.Add(cdnObject.UniqueId, cdnEntry);

            await WriteCdnMapToMetadataFile().ConfigureAwait(false);

            return cdnObject;
        }

        public async Task<bool> RemoveExpiry(CdnObject link)
        {
            if (cdnMetadata.TryGetValue(link.UniqueId, out var cdnEntry))
            {
                cdnEntry.ExpiresAt = null;

                await WriteCdnMapToMetadataFile().ConfigureAwait(false);

                return true;
            }

            return false;
        }

        public async Task SetExpiry(CdnObject link, Instant expireAt)
        {
            if (cdnMetadata.TryGetValue(link.UniqueId, out var cdnEntry))
            {
                cdnEntry.ExpiresAt = expireAt;

                await WriteCdnMapToMetadataFile().ConfigureAwait(false);
            }
        }

        public async Task<bool> TryDeleteObject(CdnObject link)
        {
            var cdnDir = Path.Combine(cdnPath, link.UniqueId);

            if (!Directory.Exists(cdnDir))
            {
                return false;
            }

            Directory.Delete(cdnDir, true);

            _ = cdnMetadata.Remove(link.UniqueId);

            logger.Information("CDN object {0} has been deleted.", link.UniqueId);

            await WriteCdnMapToMetadataFile().ConfigureAwait(false);

            return true;
        }

        public Task<Uri> GetObjectActualUrl(CdnObject link)
        {
            // REVIEW: Should I throw on a null HttpContext
            if (httpContextAccessor.HttpContext is null)
            {
                logger.Error("The HttpContext is somehow null, cannot obtain the download URL.");
                throw new InvalidOperationException(nameof(httpContextAccessor.HttpContext));
            }

            // Get components to construct the base url
            var request = httpContextAccessor.HttpContext.Request;
            var host = request.Host.ToUriComponent();
            var pathBase = request.PathBase.ToUriComponent();
            // REVIEW: Should I enforce HTTPS here or stick with the request scheme
            var baseUrl = $"{request.Scheme}://{host}{pathBase}";

            // Get CDN unique ID and object name
            var cdnUniqueId = link.UniqueId;
            var cdnEntry = cdnMetadata[cdnUniqueId];

            // Slap everything together and return the result
            var cdnUrl = $"{baseUrl}/{cdnSubfolder}/{cdnUniqueId}/{cdnEntry.ObjectName}";

            return Task.FromResult(new Uri(cdnUrl));
        }

        public Task<string> GetObjectName(CdnObject link)
            => cdnMetadata.TryGetValue(link.UniqueId, out var cdnEntry)
                ? Task.FromResult(cdnEntry.ObjectName)
                // REVIEW: What do return if CdnObject isn't in CDN
                : Task.FromResult(string.Empty);

        // Helper method to write our cdn object to the metadata file.
        private async Task WriteCdnMapToMetadataFile()
        {
            using var metadataStream = File.OpenWrite(metadataPath);

            await JsonSerializer.SerializeAsync(metadataStream, cdnMetadata).ConfigureAwait(false);
        }
    }
}
