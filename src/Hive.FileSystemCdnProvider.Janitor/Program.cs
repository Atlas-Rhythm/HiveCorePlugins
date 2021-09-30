using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NodaTime;

namespace Hive.FileSystemCdnProvider.Janitor
{
    /*
     * The Janitor application is a simple console application which enumerates over all metadata files
     * in the Hive CDN folder, and removes any entries that are past their expire date.
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
            var cdnPath = new DirectoryInfo(args[0]);

            if (!cdnPath.Exists)
            {
                throw new DirectoryNotFoundException("The CDN directory given to the Janitor does not exist.");
            }

            var currentInstant = SystemClock.Instance.GetCurrentInstant();

            var metadataFilesToRemove = new List<string>();

            foreach (var metadataFile in cdnPath.EnumerateFiles($"*{FileSystemMetadataWrapper.MetadataExtension}"))
            {
                try
                {
                    using var stream = metadataFile.Open(FileMode.Open, FileAccess.ReadWrite);

                    var cdnEntry = await JsonSerializer.DeserializeAsync<FileSystemCdnEntry>(stream).ConfigureAwait(false);

                    if (cdnEntry.MarkedForCleanup)
                    {
                        // We add metadata files to a list to delete after we iterate here (removes stream/io jank)
                        metadataFilesToRemove.Add(metadataFile.FullName);

                        // Unique ID is the metadata file without the extension.
                        var uniqueId = metadataFile.Name.Replace(FileSystemMetadataWrapper.MetadataExtension, "");

                        // Delete the folder with the object data
                        var objectDirectory = new DirectoryInfo(Path.Combine(cdnPath.FullName, metadataFile.Name));
                        if (objectDirectory.Exists)
                        {
                            objectDirectory.Delete(true);
                        }
                    }
                    // Files can stay in CDN a little bit longer so we mark them for removal on the next sweep
                    else if (cdnEntry.ExpiresAt >= currentInstant)
                    {
                        cdnEntry.MarkedForCleanup = true;

                        await JsonSerializer.SerializeAsync(stream, cdnEntry);
                    }
                }
                catch
                {
                    // Files can stay in CDN a little bit longer, we'll get 'em next time
                    continue;
                }
            }

            // Once all metadata files are read from and accounted for, remove the ones marked for cleanup
            foreach (var metadataFile in metadataFilesToRemove)
            {
                File.Delete(metadataFile);
            }
        }
    }
}
