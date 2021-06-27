# HiveCorePlugins
This is a collection of core plugins for the extensible mod backend [Hive](https://github.com/Atlas-Rhythm/Hive), and come bundled with a standard Hive distribution.

These plugins extend Hive with basic functionality useful (or necessary) for almost all Hive instances. Because these are all plugins, you can also choose to replace these with third party plugins that provide the same functionality using different methods.

## File System Rule Provider
The File System Rule Provider plugin provides the Hive Permission System with rule information that is stored on the file system.

## Auth0 Authentication (maybe?)
The Auth0 plugin provides a basic form of authentication using [Auth0](https://auth0.com/).

## Rate Limiting (maybe?)
The Rate Limiting plugin implements the [AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit/) middleware to Hive, which offers highly extensible rate limiting.

## Tags
Tags are a collection of short keywords that are added to mods, adding new ways to group and filter related mods.

## Webhooks
The Webhooks plugin allows the broadcasting of core Hive events as a webhook.
