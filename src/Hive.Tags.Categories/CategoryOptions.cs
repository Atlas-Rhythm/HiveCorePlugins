using System;

namespace Hive.Tags.Categories;

#pragma warning disable CA1819
public class CategoryOptions
{
    public string[] Categories { get; init; } = Array.Empty<string>();
}
