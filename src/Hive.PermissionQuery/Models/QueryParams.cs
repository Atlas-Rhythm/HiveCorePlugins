using System.Collections.Generic;
using Hive.Models;

namespace Hive.PermissionQuery.Models
{
    public record QueryParams(IList<string> Actions, ModIdentifier? Mod, string? Channel);
}
