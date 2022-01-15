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
    // REVIEW: Is this the right way to go this?
    //    I asked a question a while back regarding whether or not to have constraints defined in configuration, or to use a
    //    plugin interface like the one here.
    // REVIEW: Should the default plugin remain "yes, everything is valid" or should it use the Configuration file
    //     like I described above?
    public interface ITagPlugin
    {
        /// <summary>
        /// Returns true if the provided list of tags is valid, false otherwise.
        /// </summary>
        /// <param name="tags">List of tags to validate</param>
        /// <param name="validationFailureInfo">Information about the rejection, if any.</param>
        /// <returns></returns>
        [return: StopIfReturns(false)]
        bool AreTagsValid(IList<string> tags, [ReturnLast] out object? validationFailureInfo)
        {
            validationFailureInfo = null;

            return true;
        }
    }

    public class DefaultTagPlugin : ITagPlugin { }
}
