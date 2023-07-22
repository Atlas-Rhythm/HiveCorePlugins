# Configuration

Hive.Tags.Categories uses Hive.Tags to provide category support for uploaded mods.

A list of valid categories for a mod are provided in configuration. Upon upload, if a tag in the `additionalData:Tags`
array of a mod contains a string element that starts with `category: `, it will be treated as such. It is up to
front-ends to respect the prefix. If something is prefixed with `category: ` and is not in the valid categories list
defined in the plugin configuration, the upload will be rejected.

## Example

```jsonc
"Hive.Tags.Categories": {
    "Categories": [
        "Core",
        "Libraries",
        "Tools / Tweaks"
    ]
}
```


```jsonc
// Snippet of an upload body
{
    // ...
    "additionalData": {
        "Tags": [
            "category: Libraries"
        ]
    }
}
```