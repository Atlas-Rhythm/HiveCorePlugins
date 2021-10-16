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

        private readonly FileStream fileStream;

        public FileSystemMetadataWrapper(string cdnPath, string uniqueId)
        {
            var metadataPath = Path.Combine(cdnPath, $"{uniqueId}{MetadataExtension}");

            fileStream = File.Open(metadataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            if (fileStream.Length > 0)
            {
                // REVIEW: Is this dumb?
                var bytes = new byte[fileStream.Length];
                _ = fileStream.Read(bytes, 0, bytes.Length);

                CdnEntry = JsonSerializer.Deserialize<FileSystemCdnEntry>(bytes);
            }

            _ = fileStream.Seek(0, SeekOrigin.Begin);
        }

        public async Task WriteToDisk()
        {
            fileStream.SetLength(0);

            await JsonSerializer.SerializeAsync(fileStream, CdnEntry).ConfigureAwait(false);

            _ = fileStream.Seek(0, SeekOrigin.Begin);
        }

        public void Dispose() => fileStream.Dispose();

        public ValueTask DisposeAsync() => fileStream.DisposeAsync();
    }
}
