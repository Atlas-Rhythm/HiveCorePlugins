# Permissions

Hive.Tags exposes new builtin functions to the [Permissions System](https://github.com/Atlas-Rhythm/Hive/blob/master/docs/Hive.Permissions/Usage.md).

## Tags

### `getTags(Mod)`

Returns a collection of all tags attached to the given Mod.

### `hasTag(Mod, string)`

Returns `true` if the provided Tag in the second argument is attached to the Mod in the first. Returns `false` if the provided Tag is not attached, or if the Mod contains no Tags.

### `hasAnyTags(Mod, IEnumerable<string>)`

Returns `true` if any of the provided Tags in the second argument are attached to the Mod in the first. Returns `false` if all of the provided Tags are not attached, or if the Mod contains no Tags.

### `hasAllTags(Mod, IEnumerable<string>)`

Returns `true` if *all* of the provided Tags in the second argument are attached to the Mod in the first. Returns `false` if any of the provided Tags are not attached, or if the Mod contains no Tags.

## Roles

### `getRoles(User)`

Returns a collection of all roles attached to the given User.

### `hasRole(User, string)`

Returns `true` if the provided role in the second argument is attached to the User in the first. Returns `false` if the provided role is not attached, or if the User contains no roles.

### `hasAnyRoles(User, IEnumerable<string>)`

Returns `true` if any of the provided roles in the second argument are attached to the User in the first. Returns `false` if all of the provided roles are not attached, or if the User contains no roles.

### `hasAllRoles(User, IEnumerable<string>)`

Returns `true` if *all* of the provided roles in the second argument are attached to the User in the first. Returns `false` if any of the provided roles are not attached, or if the User contains no roles.