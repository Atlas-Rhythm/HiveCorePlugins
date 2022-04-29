using System.Collections.Generic;

namespace Hive.PermissionQuery
{
    public class PermissionQueryOptions
    {
        public IList<string> WhitelistedActions { get; private set; } = new List<string>();
    }
}
