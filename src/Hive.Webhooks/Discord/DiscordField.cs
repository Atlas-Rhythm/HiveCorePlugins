﻿namespace Hive.Webhooks.Discord;

public class DiscordField
{
    public string Name { get; set; }
    public string Value { get; set; }

    public bool? Inline { get; set; }

    public DiscordField(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
