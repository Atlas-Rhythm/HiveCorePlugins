using System.Collections.Generic;
using Hive.Controllers;
using Hive.Models;
using Microsoft.Extensions.Options;

namespace Hive.AdditionalUserDataExposer;

public class AdditionalUserDataExposerPlugin : IUserPlugin
{
    private readonly IOptions<AdditionalUserDataExposerOptions> _options;

    public AdditionalUserDataExposerPlugin(IOptions<AdditionalUserDataExposerOptions> options) => _options = options;

    void IUserPlugin.ExposeUserInfo(Dictionary<string, object> data, ArbitraryAdditionalData userData)
    {
        foreach (var property in _options.Value.Properties)
        {
            if (!userData.TryGetValue(property, out var value, typeof(string)))
                continue;

            data[property] = value!;
        }
    }
}
