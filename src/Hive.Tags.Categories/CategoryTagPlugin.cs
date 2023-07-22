using System;
using System.Collections.Generic;
using System.Linq;
using Hive.Plugins.Aggregates;
using Hive.Tags.Plugins;

namespace Hive.Tags.Categories;

public class CategoryTagPlugin : ITagPlugin
{
    private readonly CategoryOptions _categoryOptions;
    private const string CategoryPrefix = "category: ";

    public CategoryTagPlugin(CategoryOptions categoryOptions) => _categoryOptions = categoryOptions;

    [return: StopIfReturns(false)]
    public bool AreTagsValid(IList<string> mutableTags, [ReturnLast] out object? validationFailureInfo)
    {
        var currentTags = mutableTags
            .Where(t => t.StartsWith(CategoryPrefix, StringComparison.InvariantCulture))
            .Select(t => t[CategoryPrefix.Length..]);

        // If any of the provided tags are not in the category list, it's a fail.
        if (!currentTags.Any(t => _categoryOptions.Categories.Contains(t)))
        {
            validationFailureInfo = "Unrecognized category tags";
            return false;
        }

        validationFailureInfo = null;
        return true;
    }
}
