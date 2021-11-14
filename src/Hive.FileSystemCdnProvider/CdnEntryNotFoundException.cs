using System;

namespace Hive.FileSystemCdnProvider
{
    public class CdnEntryNotFoundException : Exception
    {
        public CdnEntryNotFoundException(string message) : base(message)
        {
        }

        public CdnEntryNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CdnEntryNotFoundException()
        {
        }
    }
}
