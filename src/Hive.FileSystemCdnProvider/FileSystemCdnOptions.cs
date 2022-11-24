using System;

namespace Hive.FileSystemCdnProvider
{
    public class FileSystemCdnOptions
    {
        public string CdnObjectsPath { get; private set; } = "cdn/objects";
        public string CdnMetadataPath { get; private set; } = "cdn/metadata";
        public Uri? PublicUrlBase { get; private set; }
    }
}
