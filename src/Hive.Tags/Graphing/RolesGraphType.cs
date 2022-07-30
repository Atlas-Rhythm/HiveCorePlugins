using System;
using System.Linq;
using GraphQL.Types;
using Hive.Graphing;
using Hive.Graphing.Types;
using Hive.Tags.Extensions;

namespace Hive.Tags.Graphing
{
    /// <summary>
    /// The GQL representation of Roles in a <see cref="Models.User"/>.
    /// </summary>
    public class RolesGraphType : ICustomHiveGraph<UserType>
    {
        public void Configure(UserType graphType)
        {
            if (graphType == null)
            {
                throw new ArgumentNullException(nameof(graphType));
            }

            _ = graphType.Field<ListGraphType<StringGraphType>>(
                "roles",
                $"(Exposed by {nameof(Tags)} List of roles assigned to this user.",
                resolve: ctx => ctx.Source?.GetRoles() ?? Enumerable.Empty<string>());
        }
    }
}
