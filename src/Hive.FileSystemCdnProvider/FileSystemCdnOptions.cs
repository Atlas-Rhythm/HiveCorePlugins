using System;

namespace Hive.FileSystemCdnProvider
{
    public class FileSystemCdnOptions
    {
        public string CdnObjectsSubfolder { get; set; } = "cdn/objects";
        public string CdnMetadataSubfolder { get; set; } = "cdn/metadata";
        public Uri? PublicUrlBase { get; set; }
    }
}
