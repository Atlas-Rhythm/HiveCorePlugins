using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public static class Program
    {
        /*
         * Janitor arguments:
         * 0 - Path to core Hive installation
         * 1 - CDN Metadata subfolder ("CdnMetadataSubfolder" plugin config key)
         * 2 - CDN Object subfolder ("CdnObjectsSubfolder" plugin config key)
         */
        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "The Janitor does not care about exceptions, and will simply try again on next execution.")]
        public static async Task Main(string[] args)
        {
            // This should NEVER happen, but Visual Studio is yelling at me
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length is not 3)
            {
                throw new ArgumentException("The Janitor requires 3 text arguments in this order: "
                    + "The absolute path to the Hive installation, "
                    + "the relative CDN Metadata subfolder, "
                    + "and the relative CDN Objects subfolder.");
            }

            // Grab our directory infos and throw if any dont exist
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

            // Enumerate all .metadata files in the CDN Metadatas folder
            foreach (var metadataFile in cdnMetadataFolder.EnumerateFiles($"*{FileSystemMetadataWrapper.MetadataExtension}"))
            {
                try
                {
                    using var stream = metadataFile.Open(FileMode.Open, FileAccess.ReadWrite);

                    var cdnEntry = await JsonSerializer.DeserializeAsync<FileSystemCdnEntry>(stream).ConfigureAwait(false);

                    // Does the CDN Entry not exist?
                    if (cdnEntry == null)
                    {
                        // If it does not, our metadata file points to nonexistent data, we should clean that up.
                        metadataFilesToRemove.Add(metadataFile.FullName);

                        continue;
                    }

                    if (cdnEntry.MarkedForCleanup)
                    {
                        // Unique ID is the metadata file without the extension.
                        var uniqueId = metadataFile.Name.Replace(FileSystemMetadataWrapper.MetadataExtension, "",
                            StringComparison.InvariantCultureIgnoreCase);

                        // Delete the folder with the object data
                        var objectDirectory = new DirectoryInfo(Path.Combine(cdnObjectsFolder.FullName, uniqueId));
                        if (objectDirectory.Exists)
                        {
                            objectDirectory.Delete(true);
                        }

                        // Only flag metadata file for removal after CDN objects were successfully removed.
                        metadataFilesToRemove.Add(metadataFile.FullName);
                    }
                    // Files can stay in CDN a little bit longer so we mark them for removal on the next sweep
                    else if (cdnEntry.ExpiresAt >= currentInstant)
                    {
                        cdnEntry.MarkedForCleanup = true;

                        await JsonSerializer.SerializeAsync(stream, cdnEntry).ConfigureAwait(false);
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
                try
                {
                    File.Delete(metadataFile);
                }
                catch
                {
                    // Metadata files can also stay in CDN a little longer, we'll get 'em next time
                    continue;
                }
            }
        }
    }
}
