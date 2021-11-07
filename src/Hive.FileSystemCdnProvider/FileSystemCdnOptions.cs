using System;

namespace Hive.FileSystemCdnProvider
{
    public class FileSystemCdnOptions
    {
        public string CdnObjectsSubfolder { get; private set; } = "cdn/objects";
        public string CdnMetadataSubfolder { get; private set; } = "cdn/metadata";
        public Uri? PublicUrlBase { get; private set; }
    }
}
