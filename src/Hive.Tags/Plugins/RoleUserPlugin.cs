using System;
using System.Collections.Generic;
using Hive.Controllers;
using Hive.Models;
using Hive.Tags.Extensions;

namespace Hive.Tags.Plugins
{
    public class RoleUserPlugin : IUserPlugin
    {
        /// <inheritdoc/>
        public void ExposeUserInfo(Dictionary<string, object> data, ArbitraryAdditionalData userData)
        {
            // REVIEW: Is there merit to throwing an exception rather than peacefully exiting?
            if (userData is null)
            {
                throw new ArgumentNullException(nameof(userData));
            }

            data ??= new();

            if (userData.TryGetValue<List<string>>(HiveModelExtensions.RolesAdditionalDataKey, out var roles)
                && roles != null)
            {
                data.Add(HiveModelExtensions.RolesAdditionalDataKey, roles);
            }
        }
    }
}
