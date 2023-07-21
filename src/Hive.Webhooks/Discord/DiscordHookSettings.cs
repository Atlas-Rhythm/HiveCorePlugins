using System;
using System.Text.Json.Serialization;

namespace Hive.Webhooks.Discord;

public class DiscordHookSettings
{
    public string CreatedColor { get; init; } = "#00C28C";

    public string NeutralColor { get; init; } = "#4287f5";

    private int? _createdColor;
    [JsonIgnore]
    public int CreatedColorValue
    {
        get
        {
            _createdColor ??= Convert.ToInt32(CreatedColor.Replace("#", string.Empty, StringComparison.InvariantCulture), 16);
            return _createdColor.GetValueOrDefault();
        }
    }

    private int? _neutralColor;
    [JsonIgnore]
    public int NeutralColorValue
    {
        get
        {
            _neutralColor ??= Convert.ToInt32(NeutralColor.Replace("#", string.Empty, StringComparison.InvariantCulture), 16);
            return _neutralColor.GetValueOrDefault();
        }
    }
}
