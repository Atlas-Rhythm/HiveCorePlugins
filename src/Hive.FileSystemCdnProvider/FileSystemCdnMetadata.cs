using System.Collections.Generic;

namespace Hive.FileSystemCdnProvider
{
    /// <summary>
    /// Represents the metadata file for the File System CDN.
    /// 
    /// Currently, it's just a renamed <see cref="Dictionary{string, FileSystemCdnEntry}"/>,
    /// but any useful methods/abstractions can be added directly to this class.
    /// </summary>
    // REVIEW: Would this de/serialize correctly with System.Text.Json?
    public class FileSystemCdnMetadata : Dictionary<string, FileSystemCdnEntry>
    {
    }
}
