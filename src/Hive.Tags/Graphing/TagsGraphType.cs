using System;
using System.Linq;
using GraphQL.Types;
using Hive.Graphing;
using Hive.Graphing.Types;
using Hive.Tags.Extensions;

namespace Hive.Tags.Graphing
{
    /// <summary>
    /// The GQL representation of Tags in a <see cref="Hive.Models.Mod"/>.
    /// </summary>
    public class TagsGraphType : ICustomHiveGraph<ModType>
    {
        public void Configure(ModType graphType)
        {
            if (graphType == null)
            {
                throw new ArgumentNullException(nameof(graphType));
            }

            // REVIEW: Should this field be at the top level, or should I wrap it in an "additionalData" object?
            //    Alternatively, should this be replaced by a blanket "additionalData" GQL object on Hive's end?
            // REVIEW: Localize description field?
            _ = graphType.Field<ListGraphType<StringGraphType>>(
                "tags",
                "(Exposed by Hive.Tags) List of tags assigned to this mod.",
                resolve: ctx => ctx.Source!.GetTags()?.Tags ?? Enumerable.Empty<string>());
        }
    }
}
