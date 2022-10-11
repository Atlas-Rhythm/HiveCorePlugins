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

            container.RegisterInstance<(string, Delegate)>(("getRoles", GetRoles));

            container.RegisterInstance<(string, Delegate)>(("hasRole", HasRole));

            // TODO: MathExpr does not currently support sequences, so the below functions may not be accessible
            container.RegisterInstance<(string, Delegate)>(("hasAnyTags", HasAnyTag));

            container.RegisterInstance<(string, Delegate)>(("hasAllTags", HasAllTags));

            container.RegisterInstance<(string, Delegate)>(("hasAnyRoles", HasAnyRole));

            container.RegisterInstance<(string, Delegate)>(("hasAllRoles", HasAllRoles));
        }

        // Returns true iff the mod has the given tag
        private static bool HasTag(Mod mod, string tag) => GetTags(mod).Contains(tag);

        // Returns true iff the mod has any of the given tags
        private static bool HasAnyTag(Mod mod, IEnumerable<string> tags) => GetTags(mod).Any(t => tags.Contains(t));

        // Returns true iff the mod has all of the given tags
        private static bool HasAllTags(Mod mod, IEnumerable<string> tags)
        {
            var assignedTags = GetTags(mod);

            return tags.All(tag => assignedTags.Contains(tag));
        }

        // Retrieves all tags attached to the given mod
        private static IEnumerable<string> GetTags(Mod mod) => mod.GetTags() ?? Enumerable.Empty<string>();

        // Returns true iff the user has the given role
        private static bool HasRole(User user, string role) => GetRoles(user).Contains(role);

        // Returns true iff the user has any of the given roles
        private static bool HasAnyRole(User user, IEnumerable<string> roles) => GetRoles(user).Any(t => roles.Contains(t));

        // Returns true iff the user has all of the given roles
        private static bool HasAllRoles(User mod, IEnumerable<string> roles)
        {
            var assignedTags = GetRoles(mod);

            return roles.All(tag => assignedTags.Contains(tag));
        }

        // Retrieves all roles attached to the given user
        private static IEnumerable<string> GetRoles(User user) => user.GetRoles() ?? Enumerable.Empty<string>();
    }
}
