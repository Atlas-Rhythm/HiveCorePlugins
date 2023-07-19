# Configuration

Hive.AdditionalUserDataExposer exposes user data properties defined by the instance configuration.

## Example
```jsonc
"Hive.AdditionalUserDataExposer": {
    "Properties": [
        "picture" // Exposes the picture property on the user, which is not serialized with returned user objects by default.
    ]
}
```