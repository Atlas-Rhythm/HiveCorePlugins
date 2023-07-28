#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build