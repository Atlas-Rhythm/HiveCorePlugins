using System.Collections.Generic;
using Hive.Plugins.Aggregates;

namespace Hive.Tags.Plugins
{
    /// <summary>
    /// A class for plugins that allow modifications of Tags validation
    /// </summary>
    /// <remarks>
    /// Exposed plugin interface for a plugin, I know.
    /// </remarks>
    [Aggregable(Default = typeof(DefaultTagPlugin))]
    public interface ITagPlugin
    {
        /// <summary>
        /// Returns <c>true</c> if the provided list of tags is valid, <c>false</c> otherwise.
        /// </summary>
        /// <remarks>
        /// If the plugin can safely fix an otherwise invalid list of tags, the plugin should make a good attempt
        /// at modifying <paramref name="mutableTags"/> and return <c>true</c> on a successful fix.
        /// </remarks>
        /// <param name="mutableTags">List of mutable tags to validate.</param>
        /// <param name="validationFailureInfo">Information about the rejection, if any.</param>
        /// <returns>Whether or not the provided list of tags is valid.</returns>
        [return: StopIfReturns(false)]
        bool AreTagsValid(IList<string> mutableTags, [ReturnLast] out object? validationFailureInfo)
        {
            validationFailureInfo = null;

            return true;
        }
    }

    public class DefaultTagPlugin : ITagPlugin { }
}
