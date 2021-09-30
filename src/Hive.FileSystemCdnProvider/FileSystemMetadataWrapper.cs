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

        public FileSystemCdnEntry? CdnEntry { get; set; }

        public string MetadataPath { get; init; }

        private readonly FileStream fileStream;

        public FileSystemMetadataWrapper(string cdnPath, string uniqueId)
            : this(Path.Combine(cdnPath, $"{uniqueId}{MetadataExtension}"))
        {
        }

        public FileSystemMetadataWrapper(string completeMetadataPath)
        {
            MetadataPath = completeMetadataPath;

            fileStream = File.Open(completeMetadataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            // REVIEW: Is this dumb?
            var bytes = new byte[fileStream.Length];
            _ = fileStream.Read(bytes, 0, bytes.Length);

            CdnEntry = JsonSerializer.Deserialize<FileSystemCdnEntry>(bytes);
        }

        public async Task WriteToDisk() => await JsonSerializer.SerializeAsync(fileStream, CdnEntry).ConfigureAwait(false);

        public void Dispose() => fileStream.Dispose();

        public ValueTask DisposeAsync() => fileStream.DisposeAsync();
    }
}
