# !!!
# THIS DOCKERFILE IS NOT MEANT TO BE DEPLOYED!
#
# This dockerfile is meant to build plugins for the main Hive backend (github.com/Atlas-Rhythm/Hive)
#!!!


# Restore and build the project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o

# Copy project artifacts to a staging directory.
# The Hive dockerfile will copy these plugins to its own image.
# REVIEW: my docker experience is pretty minimal so im not sure if this is the best solution for this
FROM build AS format-hive-plugins
COPY ["artifacts/bin/Hive.AdditionalUserDataExposer/Release/net6.0/", "Plugins/Hive.AdditionalUserDataExposer/"]
COPY ["artifacts/bin/Hive.FileSystemCdnProvider/Release/net6.0/", "Plugins/Hive.FileSystemCdnProvider/"]
COPY ["artifacts/bin/Hive.FileSystemRuleProvider/Release/net6.0/", "Plugins/Hive.FileSystemRuleProvider/"]
COPY ["artifacts/bin/Hive.PermissionQuery/Release/net6.0/", "Plugins/Hive.PermissionQuery/"]
COPY ["artifacts/bin/Hive.RateLimiting/Release/net6.0/", "Plugins/Hive.RateLimiting/"]
COPY ["artifacts/bin/Hive.Tags/Release/net6.0/", "Plugins/Hive.Tags/"]
COPY ["artifacts/bin/Hive.Tags.Categories/Release/net6.0/", "Plugins/Hive.Tags.Categories/"]
COPY ["artifacts/bin/Hive.Webhooks/Release/net6.0/", "Plugins/Hive.Webhooks/"]