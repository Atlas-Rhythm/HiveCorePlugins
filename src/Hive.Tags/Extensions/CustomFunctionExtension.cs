using DryIoc;
using Hive.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hive.Tags.Extensions
{
    /// <summary>
    /// Extension class which implements custom built-in functions for the Permissions System.
    /// </summary>
    public static class CustomFunctionExtension
    {
        public static void RegisterCustomFunctions(this IContainer container)
        {
            container.RegisterInstance<(string, Delegate)>(("getTags", GetTags));

            container.RegisterInstance<(string, Delegate)>(("hasTag", HasTag));

            // TODO: MathExpr does not currently support sequences, so the below functions may not be accessible
            container.RegisterInstance<(string, Delegate)>(("hasAnyTags", HasAnyTag));

            container.RegisterInstance<(string, Delegate)>(("hasAllTags", HasAllTags));
        }

        // Returns true iff the mod has the given tag
        private static bool HasTag(Mod mod, string tag) => GetTags(mod).Contains(tag);

        // Returns true iff the mod has any of the given tags
        private static bool HasAnyTag(Mod mod, IEnumerable<string> tags) => GetTags(mod).Any(t => tags.Contains(t));

        // Returns true iff the mod has all of the given tags
        private static bool HasAllTags(Mod mod, IEnumerable<string> tags)
        {
            var assignedTags = GetTags(mod);

            // REVIEW: This returns true if "tags" is empty. Should this be the case?
            return tags.All(tag => assignedTags.Contains(tag));
        }

        // Retrieves all tags attached to the given mod
        private static IEnumerable<string> GetTags(Mod mod) => mod.GetTags()?.Tags ?? Enumerable.Empty<string>();
    }
}
