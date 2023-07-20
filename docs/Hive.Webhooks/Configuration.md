# Permissions

Hive.Webhooks allows sending webhook events when certain events happen.

It will automatically emit events for the following:
* Game Version Created
* Channel Created
* Mod Uploaded
* Mod Moved

## Example

```jsonc
"Hive.Webhooks": {
    "EmitTo": {
        "Discord": [
            "<YOUR WEBHOOK TARGET URL>"
        ]
    },
    "Discord": {
        "CreatedColor": "#00C28C",
        "NeutralColor": "#4287f5"
    }
}
```