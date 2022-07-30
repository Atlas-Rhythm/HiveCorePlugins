using System;
using System.Linq;
using GraphQL.Types;
using Hive.Graphing;
using Hive.Graphing.Types;
using Hive.Tags.Extensions;

namespace Hive.Tags.Graphing
{
    /// <summary>
    /// The GQL representation of Tags in a <see cref="Models.Mod"/>.
    /// </summary>
    public class TagsGraphType : ICustomHiveGraph<ModType>
    {
        public void Configure(ModType graphType)
        {
            if (graphType == null)
            {
                throw new ArgumentNullException(nameof(graphType));
            }

            _ = graphType.Field<ListGraphType<StringGraphType>>(
                "tags",
                $"(Exposed by {nameof(Tags)}) List of tags assigned to this mod.",
                resolve: ctx => ctx.Source?.GetTags() ?? Enumerable.Empty<string>());
        }
    }
}
