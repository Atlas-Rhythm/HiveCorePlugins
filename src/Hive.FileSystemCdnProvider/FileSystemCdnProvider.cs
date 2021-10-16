using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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

        private readonly ILogger logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string cdnSubfolder;
        private readonly string cdnPath;

        public FileSystemCdnProvider(ILogger logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;

            cdnSubfolder = configuration.GetValue(SubfolderConfigurationKey, SubfolderDefaultValue);

            cdnPath = Path.Combine(Directory.GetCurrentDirectory(), cdnSubfolder);

            _ = Directory.CreateDirectory(cdnPath);
        }

        public async Task<CdnObject> UploadObject(string name, Stream data, Instant? expireAt)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            logger.Information("Uploading {0} to File System CDN...", name);

            // Unique ID will be a new GUID
            var uniqueId = Guid.NewGuid().ToString();

            // Create our metadata file here, with our new unique ID
            using var metadata = new FileSystemMetadataWrapper(cdnPath, uniqueId);

            // The UploadController already seeks to the beginning, but this is just in case another caller forgets to
            _ = data.Seek(0, SeekOrigin.Begin);

            // CDN directory shall be <Hive>/<Subfolder>/<Stream Hash>
            var objectCdnDirectory = Path.Combine(cdnPath, uniqueId);
            _ = Directory.CreateDirectory(objectCdnDirectory);

            // Lets construct a file stream and copy our data to that file.
            var objectCdnPath = Path.Combine(objectCdnDirectory, name);
            using var fileStream = File.Create(objectCdnPath);

            await data.CopyToAsync(fileStream).ConfigureAwait(false);

            // Construct CDN data and assign it to the metadata stream.
            var cdnObject = new CdnObject(uniqueId);
            metadata.CdnEntry = new FileSystemCdnEntry(name, expireAt);

            // Write to disk
            await metadata.WriteToDisk().ConfigureAwait(false);

            return cdnObject;
        }

        public async Task<bool> RemoveExpiry(CdnObject link)
        {
            // Load metadata file from disk.
            using var metadata = new FileSystemMetadataWrapper(cdnPath, link.UniqueId);

            // Return false if not found
            if (metadata.CdnEntry is null)
                return false;

            // Wipe expire date and reset cleanup mark
            metadata.CdnEntry.ExpiresAt = null;
            metadata.CdnEntry.MarkedForCleanup = false;

            // Write to disk
            await metadata.WriteToDisk().ConfigureAwait(false);

            return true;
        }

        public async Task SetExpiry(CdnObject link, Instant expireAt)
        {
            // Load metadata file from disk.
            using var metadata = new FileSystemMetadataWrapper(cdnPath, link.UniqueId);

            // Throw if not found
            if (metadata.CdnEntry is null)
                throw new CdnEntryNotFoundException(nameof(link.UniqueId));

            // Set expiry and reset cleanup mark
            metadata.CdnEntry.ExpiresAt = expireAt;
            metadata.CdnEntry.MarkedForCleanup = false;

            // Write to disk
            await metadata.WriteToDisk().ConfigureAwait(false);
        }

        // REVIEW: Is this stupid (both the suppression and not being completely async)
        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = @"Directory.Delete and File.Delete can throw many exceptions, but the behavior for each is the same.
                                Furthermore, the docs say to return false if the operation fails, rather than throw an exception.")]
        public Task<bool> TryDeleteObject(CdnObject link)
        {
            // Construct metadata path ourselves so we're not pinging disk
            var metadataFile = Path.Combine(cdnPath, $"{link.UniqueId}{FileSystemMetadataWrapper.MetadataExtension}");

            // Get directory that contains object
            var cdnDir = Path.Combine(cdnPath, link.UniqueId);

            try
            {
                Directory.Delete(cdnDir, true);
                File.Delete(metadataFile);

                logger.Information("CDN object {0} has been deleted.", link.UniqueId);

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                logger.Error("CDN object {0} failed to be deleted: {1}", link.UniqueId, e);

                return Task.FromResult(false);
            }
        }

        public Task<Uri> GetObjectActualUrl(CdnObject link)
        {
            // Load metadata file from disk.
            using var metadata = new FileSystemMetadataWrapper(cdnPath, link.UniqueId);

            // Throw if not found
            if (metadata.CdnEntry is null)
                throw new CdnEntryNotFoundException(nameof(link.UniqueId));

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

            // Slap everything together and return the result
            var cdnUrl = $"{baseUrl}/{cdnSubfolder}/{cdnUniqueId}/{metadata.CdnEntry.ObjectName}";

            return Task.FromResult(new Uri(cdnUrl));
        }

        public async Task<string> GetObjectName(CdnObject link)
        {
            // Load metadata from disk
            using var metadata = new FileSystemMetadataWrapper(cdnPath, link.UniqueId);

            // Obtain object name (or throw if not found)
            var objectName = metadata.CdnEntry?.ObjectName ?? throw new CdnEntryNotFoundException(nameof(link.UniqueId));

            // Write metadata to disk
            await metadata.WriteToDisk().ConfigureAwait(false);

            return objectName;
        }
    }
}
