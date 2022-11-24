# Configuration

A key component to Hive is its permission system, and the rules that govern it.

This page is for the configuration of Hive's permission system. [Please visit this link for documentation on the permission system itself.](https://github.com/Atlas-Rhythm/Hive/blob/master/Hive.Permissions/docs/Usage.md).

## `RulePath` - `string`

This determines the path (absolute or relative) where permission rules will be read from. By default, this is set to `Rules`.

# File System Structure

Permission rules are stored in the Hive installation folder, in the subfolder defined in the `RuleSubfolder` configuration entry.

Each rule's definition is stored as a `.rule` file inside this subfolder. To determine a rule's location on the file system, the plugin breaks the action into its individual identifiers. The *last* identifier will always be used as the file name, and all preceding identifiers will be used as additional subfolders for more organization. Here are a couple of examples:
- `hive` -> `hive.rule`
- `hive.mod` -> `hive/mod.rule`
- `hive.mod.edit` -> `hive/mod/edit.rule`
- `hive.mod.additionalData` -> `hive/mod/additionalData.rule`

When all of these rules are put together, the file structure would look like such:

```
| hive.rule
| hive/
|-> mod.rule
|-> mod/
|---> edit.rule
|---> additionalData.rule
```

# Rule Behavior

Any and all rules *must* be defined in the file system. If Hive.FileSystemRuleProvider does not find the `.rule` file for a particular rule, the plugin will note its absence in log, and assume that particular rule did not exist.