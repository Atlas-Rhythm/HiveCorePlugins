using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Hive.Controllers;
using Hive.Models;
using Hive.Tags.Extensions;

namespace Hive.Tags.Plugins
{
    public class RoleUserPlugin : IUserPlugin
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "This is an interface method from IUserPlugin")]
        /// <inheritdoc/>
        public void ExposeUserInfo(Dictionary<string, object> data, ArbitraryAdditionalData userData)
        {
            // REVIEW: Is there merit to throwing an exception rather than peacefully exiting?
            if (userData is null)
            {
                throw new ArgumentNullException(nameof(userData));
            }

            data ??= new();

            var roles = userData.Get<List<string>>(HiveModelExtensions.RolesAdditionalDataKey);

            if (roles != null)
            {
                data.Add(HiveModelExtensions.RolesAdditionalDataKey, roles);
            }
        }
    }
}
