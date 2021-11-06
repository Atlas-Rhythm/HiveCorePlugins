﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Hive.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NodaTime;
using Serilog;

namespace Hive.FileSystemCdnProvider
{
    public class FileSystemCdnProvider : ICdnProvider
    {
        private readonly ILogger logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string? publicUrlBase;

        private readonly string cdnObjectSubfolder;
        private readonly string cdnObjectPath;

        private readonly string cdnMetadataSubfolder;
        private readonly string cdnMetadataPath;

        public FileSystemCdnProvider(ILogger logger, IHttpContextAccessor httpContextAccessor, IOptions<FileSystemCdnOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;

            cdnObjectSubfolder = options.Value.CdnObjectsSubfolder;
            cdnMetadataSubfolder = options.Value.CdnMetadataSubfolder;

            publicUrlBase = options.Value.PublicUrlBase?.ToString();

            cdnObjectPath = Path.Combine(Directory.GetCurrentDirectory(), cdnObjectSubfolder);
            cdnMetadataPath = Path.Combine(Directory.GetCurrentDirectory(), cdnMetadataSubfolder);

            _ = Directory.CreateDirectory(cdnObjectPath);
            _ = Directory.CreateDirectory(cdnMetadataPath);
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
            using var metadata = await FileSystemMetadataWrapper.OpenMetadataAsync(cdnMetadataPath, uniqueId).ConfigureAwait(false);

            // CDN directory shall be <Hive>/<Object Subfolder>/<Stream Hash>
            var objectCdnDirectory = Path.Combine(cdnObjectPath, uniqueId);
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
            using var metadata = await FileSystemMetadataWrapper.OpenMetadataAsync(cdnMetadataPath, link.UniqueId).ConfigureAwait(false);

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
            using var metadata = await FileSystemMetadataWrapper.OpenMetadataAsync(cdnMetadataPath, link.UniqueId).ConfigureAwait(false);

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
        public async Task<bool> TryDeleteObject(CdnObject link)
        {
            // Construct metadata path ourselves so we're not pinging disk
            var metadataFile = Path.Combine(cdnMetadataPath, $"{link.UniqueId}{FileSystemMetadataWrapper.MetadataExtension}");

            // Get directory that contains object
            var cdnDir = Path.Combine(cdnObjectPath, link.UniqueId);

            try
            {
                Directory.Delete(cdnDir, true);
                File.Delete(metadataFile);

                logger.Information("CDN object {0} has been deleted.", link.UniqueId);

                return true;
            }
            catch (Exception e)
            {
                logger.Error("CDN object {0} failed to be deleted: {1}", link.UniqueId, e);

                using var metadata = await FileSystemMetadataWrapper.OpenMetadataAsync(cdnMetadataPath, link.UniqueId).ConfigureAwait(false);

                // If the CDN Entry is null, I guess the object was already deleted?
                // Otherwise, we will mark the file for cleanup and have the Janitor deal with it.
                if (metadata.CdnEntry != null)
                {
                    logger.Warning("Marking CDN object {0} for Janitor cleanup.", link.UniqueId);

                    metadata.CdnEntry.MarkedForCleanup = true;

                    await metadata.WriteToDisk().ConfigureAwait(false);
                }

                return false;
            }
        }

        public async Task<Uri> GetObjectActualUrl(CdnObject link)
        {
            // Load metadata file from disk.
            using var metadata = await FileSystemMetadataWrapper.OpenMetadataAsync(cdnMetadataPath, link.UniqueId).ConfigureAwait(false);

            // Throw if not found
            if (metadata.CdnEntry is null)
                throw new CdnEntryNotFoundException(nameof(link.UniqueId));

            // REVIEW: Should I throw on a null HttpContext
            if (httpContextAccessor.HttpContext is null)
            {
                logger.Error("The HttpContext is somehow null, cannot obtain the download URL.");
                throw new InvalidOperationException(nameof(httpContextAccessor.HttpContext));
            }

            var baseUrl = publicUrlBase;

            if (baseUrl is null)
            {
                // Get components to construct the base url
                var request = httpContextAccessor.HttpContext.Request;
                var host = request.Host.ToUriComponent();
                var pathBase = request.PathBase.ToUriComponent();

                // REVIEW: Should I enforce HTTPS here or stick with the request scheme
                baseUrl = $"{request.Scheme}://{host}{pathBase}";
            }

            // Get CDN unique ID and object name
            var cdnUniqueId = link.UniqueId;

            // Slap everything together and return the result
            // REVIEW: Should I include cdnSubfolder if we are using a public URL base (for reverse proxies, etc.)?
            var cdnUrl = $"{baseUrl}/{cdnObjectSubfolder}/{cdnUniqueId}/{metadata.CdnEntry.ObjectName}";

            return new Uri(cdnUrl);
        }

        public async Task<string> GetObjectName(CdnObject link)
        {
            // Load metadata from disk
            using var metadata = await FileSystemMetadataWrapper.OpenMetadataAsync(cdnMetadataPath, link.UniqueId).ConfigureAwait(false);

            // Return object name (or throw if not found)
            return metadata.CdnEntry?.ObjectName ?? throw new CdnEntryNotFoundException(nameof(link.UniqueId));
        }
    }
}
