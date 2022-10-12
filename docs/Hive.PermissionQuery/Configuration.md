# Configuration

Permission Query allows external sources to query Hive's Permission System.

## `WhitelistedActions` - `string[]`

This defines what actions can be queried through the Permission Query endpoint. If this is empty, it is assumed that *everything* is available for querying.