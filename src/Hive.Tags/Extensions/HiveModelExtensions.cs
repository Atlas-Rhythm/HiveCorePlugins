using System;
using Hive.Models;
using Hive.Tags.Models;

namespace Hive.Tags.Extensions
{
    public static class HiveModelExtensions
    {
        /// <summary>
        /// The key in <see cref="ArbitraryAdditionalData"/> from which tags are stored/retrieved.
        /// </summary>
        // Using "Hive_Tags" instead of "Hive.Tags" to hopefully prevent "ctx.Mod.AdditionalData.Hive.Tags...."
        public const string AdditionalDataKey = "Hive_Tags";

        /// <summary>
        /// Retrieves all tag information attached to the specified <see cref="Mod"/>.
        /// </summary>
        /// <remarks>
        /// This is a deserialized object, and changes do not immediately reflect in the <see cref="Mod"/> object.
        /// You must commit new changes with <see cref="SetTags(Mod, TagsAdditionalData)"/>.
        /// </remarks>
        /// <param name="mod">The mod to extract tag information from.</param>
        /// <returns>The tag information attached to a mod (or <see cref="null"/> if none exist).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mod"/> is null.</exception>
        public static TagsAdditionalData? GetTags(this Mod mod)
            => mod is not null
            ? mod.AdditionalData.Get<TagsAdditionalData>(AdditionalDataKey)
            : throw new ArgumentNullException(nameof(mod));

        /// <summary>
        /// Attaches and commits tag information to the <see cref="Mod"/>.
        /// </remarks>
        /// <param name="mod">The mod to commit tag information to.</param>
        /// <param name="tags">Tag information to attach to the mod.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mod"/> is null.</exception>
        public static void SetTags(this Mod mod, TagsAdditionalData tags)
        {
            if (mod is null) throw new ArgumentNullException(nameof(mod));

            mod.AdditionalData.Set(AdditionalDataKey, tags);
        }
    }
}
