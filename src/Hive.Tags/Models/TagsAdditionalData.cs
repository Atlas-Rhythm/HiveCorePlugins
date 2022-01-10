using System.Collections.Generic;

namespace Hive.Tags.Models
{
    /// <summary>
    /// Data structure representing Tag information inside an object.
    /// </summary>
    public class TagsAdditionalData
    {
        /// <summary>
        /// A list of tags assigned to a Hive AdditionalData object.
        /// </summary>
        /// <remarks>
        /// At first glance this seems redudant, but this design decision was made to future-proof the data structure.
        /// </remarks>
        public IList<string> Tags { get; init; } = new List<string>();
    }
}
