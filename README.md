# HiveCorePlugins
This is a collection of core plugins for the extensible mod backend [Hive](https://github.com/Atlas-Rhythm/Hive), and come bundled with a standard Hive distribution.

These plugins extend Hive with basic functionality useful (or necessary) for almost all Hive instances. Because these are all plugins, you can also choose to replace these with third party plugins that provide the same functionality using different methods.

## File System Rule Provider
The File System Rule Provider plugin provides the Hive Permission System with rule information that is stored on the file system.

## File System CDN Provider
The File System CDN Provider plugin offers a basic CDN interface which uses the local file system to store uploaded mods.

## Rate Limiting
The Rate Limiting plugin implements the [AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit/) middleware to Hive, which offers highly extensible rate limiting.

## Tags
The Tags plugin adds a collection of short keywords to mods and users, adding new ways for grouping and filtering. 

## Permission Query
The Permission Query plugin exposes the `GET api/permissions/query` endpoint and allows external sources (such as a frontend) to query the instance's Permission System to dynamically gauge the permissions of a user.

## Webhooks
The Webhooks plugin allows the broadcasting of core Hive events as a webhook.
