using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NodaTime;

namespace Hive.FileSystemCdnProvider.Janitor
{
    /*
     * The Janitor application is a simple console application which enumerates over the entries
     * in the Hive FileSystemCdnProvider metadata file, and removes any entries that are past their expire date.
     * 
     * This was designed to be a separate application which runs on a crontable, separate from main Hive.
     */
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // I don't see any need for no arguments or >1 arguments
            if (args.Length is 0 or > 1)
            {
                throw new ArgumentException("Only one argument can be passed to the Janitor.");
            }

            // Attempt to load the metadata file and throw if it does not exist
            var metadataFileInfo = new FileInfo(args[0]);

            if (!metadataFileInfo.Exists)
            {
                throw new InvalidOperationException("The metadata file given to the Janitor does not exist.");
            }

            // Open a read/write stream to the metadata file
            // REVIEW: Given Hive can possibly be reading/writing to this file at the same time, should I have the Janitor wait until the file is unlocked?
            using var fileStream = metadataFileInfo.Open(FileMode.Open, FileAccess.ReadWrite);

            // Deserialize metadata and grab our current instant to compare against the entries
            var metadata = await JsonSerializer.DeserializeAsync<FileSystemCdnMetadata>(fileStream).ConfigureAwait(false);
            var currentInstant = SystemClock.Instance.GetCurrentInstant();

            // We also need this to be a separate collection (to prevent "Collection was modified" exceptions) so we allocate a new array
            foreach (var kvp in metadata.ToArray())
            {
                // Key is CdnObject UniqueId, value is FileSystemCdnEntry (data struct which holds object name and expiry date)
                var uniqueId = kvp.Key;
                var entry = kvp.Value;

                // REVIEW: Will evaluating "Instant >= Instant?" always return false if the right hand is null?
                if (currentInstant >= entry.ExpiresAt)
                {
                    // Attempt to grab the directory that hosts the CdnObject
                    // Since the CDN entry is expired, we need to clean up its presence in the file system.
                    var cdnDirectory = Path.Combine(metadataFileInfo.Directory.FullName, uniqueId);
                    var cdnDirectoryInfo = new DirectoryInfo(cdnDirectory);

                    if (cdnDirectoryInfo.Exists)
                    {
                        cdnDirectoryInfo.Delete(true);
                    }

                    // At this point the directory should not exist any more, so we remove the uniqueID from the collection
                    _ = metadata.Remove(uniqueId);
                }
            }

            // ...And write our cleaned up results back into the metadata file.
            await JsonSerializer.SerializeAsync(fileStream, metadata);
        }
    }
}
