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
        /*
         * Janitor arguments:
         * 0 - Path to core Hive installation
         * 1 - CDN Metadata subfolder ("CdnMetadataSubfolder" plugin config key)
         * 2 - CDN Object subfolder ("CdnObjectsSubfolder" plugin config key)
         */
        public static async Task Main(string[] args)
        {
            // I don't see any need for no arguments or >1 arguments
            if (args.Length is not 3)
            {
                throw new ArgumentException("The Janitor requires 3 text arguments: The absolute path to the Hive installation, the relative CDN Metadata subfolder, and the relative CDN Objects subfolder.");
            }

            // Attempt to load the metadata file and throw if it does not exist
            var hiveInstallation = new DirectoryInfo(args[0]);
            var cdnMetadataFolder = new DirectoryInfo(Path.Combine(args[0], args[1]));
            var cdnObjectsFolder = new DirectoryInfo(Path.Combine(args[0], args[2]));

            if (!hiveInstallation.Exists)
            {
                throw new DirectoryNotFoundException($"The Hive installation folder ({hiveInstallation.FullName}) does not exist.");
            }

            if (!cdnMetadataFolder.Exists)
            {
                throw new DirectoryNotFoundException($"The given CDN Metadata folder ({cdnMetadataFolder.FullName}) does not exist.");
            }

            if (!cdnObjectsFolder.Exists)
            {
                throw new DirectoryNotFoundException($"The given CDN Objects folder ({cdnObjectsFolder.FullName}) does not exist.");
            }

            var currentInstant = SystemClock.Instance.GetCurrentInstant();

            var metadataFilesToRemove = new List<string>();

            foreach (var metadataFile in cdnMetadataFolder.EnumerateFiles($"*{FileSystemMetadataWrapper.MetadataExtension}"))
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
                        var objectDirectory = new DirectoryInfo(Path.Combine(cdnObjectsFolder.FullName, metadataFile.Name));
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
