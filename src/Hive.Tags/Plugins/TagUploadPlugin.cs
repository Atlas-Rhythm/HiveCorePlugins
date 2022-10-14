using System.IO;
using Hive.Controllers;
using Hive.Models;
using Hive.Plugins.Aggregates;
using Hive.Tags.Extensions;

namespace Hive.Tags.Plugins
{
    internal class TagUploadPlugin : IUploadPlugin
    {
        private readonly IAggregate<ITagPlugin> tagPlugins;

        public TagUploadPlugin(IAggregate<ITagPlugin> tagPlugins)
            => this.tagPlugins = tagPlugins;

        [return: StopIfReturns(false)]
        public bool ValidateAndFixUploadedData(Mod mod, ArbitraryAdditionalData originalAdditionalData, [ReturnLast] out object? validationFailureInfo)
        {
            var modTags = mod.GetTags();

            if (modTags is not null)
            {
                var aggregated = tagPlugins.Instance;

                return aggregated.AreTagsValid(modTags, out validationFailureInfo);
            }

            validationFailureInfo = null;
            return true;
        }

        // REVIEW: I am not sure if any Tag checks need to be done after data has been uploaded.
        [return: StopIfReturns(false)]
        public bool ValidateAndPopulateKnownMetadata(Mod mod, Stream data, [ReturnLast] out object? validationFailureInfo)
        {
            validationFailureInfo = null;

            return false;
        }
    }
}
