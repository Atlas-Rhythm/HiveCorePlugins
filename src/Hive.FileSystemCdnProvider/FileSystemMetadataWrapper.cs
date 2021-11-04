using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hive.FileSystemCdnProvider
{
    public sealed class FileSystemMetadataWrapper : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// The extension for all File System CDN metadata files.
        /// </summary>
        public const string MetadataExtension = ".metadata";

        /// <summary>
        /// Opens the metadata file for a entry in the file system CDN.
        /// </summary>
        /// <param name="cdnPath">Path to the CDN folder</param>
        /// <param name="uniqueId">Unique ID for a CDN object.</param>
        /// <returns>A wrapper object</returns>
        public static async Task<FileSystemMetadataWrapper> OpenMetadataAsync(string cdnPath, string uniqueId)
        {
            var metadataPath = Path.Combine(cdnPath, uniqueId + MetadataExtension);

            var fileStream = File.Open(metadataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            var cdnEntry = await JsonSerializer.DeserializeAsync<FileSystemCdnEntry>(fileStream).ConfigureAwait(false);

            return new FileSystemMetadataWrapper(fileStream, cdnEntry);
        }

        public FileSystemCdnEntry? CdnEntry { get; set; }

        private readonly FileStream fileStream;

        private FileSystemMetadataWrapper(FileStream fileStream, FileSystemCdnEntry? entry)
        {
            CdnEntry = entry;

            this.fileStream = fileStream;
        }

        // I can add this to DisposeAsync but I think it would be better if WriteToDisk must be explicitly called
        // so the behavior is clearly visible in calling methods
        public async Task WriteToDisk()
        {
            // Need to clear out the file entirely
            fileStream.SetLength(0);

            await JsonSerializer.SerializeAsync(fileStream, CdnEntry).ConfigureAwait(false);
        }

        public void Dispose() => fileStream.Dispose();

        public async ValueTask DisposeAsync() => await fileStream.DisposeAsync().ConfigureAwait(false);
    }
}
