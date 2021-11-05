# Janitor

The Janitor is a standalone .NET Console application designed to run separately from core Hive on a crontable. The Janitor iterates through all CDN metadata files, and cleans up temporary files.

## Launch Arguments

The Janitor requires 3 launch arguments (in this order) in order to successfully run.

1. The absolute path to the Hive installation
2. The Hive-relative path to the CDN Metadatas folder (in almost all cases, this is the same as the `CdnMetadataSubfolder` configuration entry)
3. The Hive-relative path to the CDN Objects folder (in almost all cases, this is the same as the `CdnObjectsSubfolder` configuration entry)

If these launch arguments are not provided, or if they point to non-existant directories, the Janitor will throw an exception and tell you what is missing.