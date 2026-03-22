# Base runtime image
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src

# Copy csproj and restore
COPY ["NotificationService/NotificationService.csproj", "./"]
RUN dotnet restore "./NotificationService.csproj"

# Copy all source files and build
COPY NotificationService/. .
RUN dotnet build "./NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "./NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]