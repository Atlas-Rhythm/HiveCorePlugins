using System;
using System.Collections.Generic;
using Hive.Models;

namespace Hive.Tags.Extensions
{
    public static class HiveModelExtensions
    {
        /// <summary>
        /// The key in <see cref="ArbitraryAdditionalData"/> from which mod tags are stored/retrieved.
        /// </summary>
        public const string TagsAdditionalDataKey = "Tags";

        /// <summary>
        /// The key in <see cref="ArbitraryAdditionalData"/> from which user roles are stored/retrieved.
        /// </summary>
        public const string RolesAdditionalDataKey = "Roles";

        /// <summary>
        /// Retrieves all tag information attached to the specified <see cref="Mod"/>.
        /// </summary>
        /// <remarks>
        /// This is a deserialized object, and changes do not immediately reflect in the <see cref="Mod"/> object.
        /// You must commit new changes with <see cref="SetTags(Mod, IList{string})"/>.
        /// </remarks>
        /// <param name="mod">The mod to extract tag information from.</param>
        /// <returns>The tags attached to a mod.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mod"/> is null.</exception>
        public static IList<string>? GetTags(this Mod mod)
            => mod is not null
            ? mod.AdditionalData.Get<List<string>>(TagsAdditionalDataKey)
            : throw new ArgumentNullException(nameof(mod));

        /// <summary>
        /// Attaches and applies tags to a given <see cref="Mod"/>.
        /// </remarks>
        /// <param name="mod">The mod to apply tags to.</param>
        /// <param name="tags">Tags to apply to the mod.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mod"/> is null.</exception>
        public static void SetTags(this Mod mod, IList<string> tags)
        {
            if (mod is null) throw new ArgumentNullException(nameof(mod));

            mod.AdditionalData.Set(TagsAdditionalDataKey, tags);
        }

        /// <summary>
        /// Retrieves all role information attached to the specified <see cref="User"/>.
        /// </summary>
        /// <remarks>
        /// This is a deserialized object, and changes do not immediately reflect in the <see cref="User"/> object.
        /// You must commit new changes with <see cref="SetRoles(User, IList{string})"/>.
        /// </remarks>
        /// <param name="user">The user to extract role information from.</param>
        /// <returns>The roles attached to the given user</returns>
        /// <exception cref="ArgumentNullException"><paramref name="user"/>is null.</exception>
        public static IList<string>? GetRoles(this User user)
            => user is not null
            ? user.AdditionalData.Get<IList<string>>(RolesAdditionalDataKey)
            : throw new ArgumentNullException(nameof(user));

        /// <summary>
        /// Attaches and applies roles to a given <see cref="User"/>.
        /// </summary>
        /// <param name="user">The user to apply roles to.</param>
        /// <param name="roles">Roles to apply to the user.</param>
        /// <exception cref="ArgumentNullException"><paramref name="user"/> is null.</exception>
        public static void SetRoles(this User user, IList<string> roles)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            user.AdditionalData.Set(RolesAdditionalDataKey, roles);
        }
    }
}
