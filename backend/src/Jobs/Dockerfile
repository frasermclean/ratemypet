# Jobs Functions App Dockerfile
# This needs to be run from the backend directory (build context)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Jobs/Jobs.csproj", "Jobs/"]
COPY ["src/Persistence/Persistence.csproj", "Persistence/"]
COPY ["src/Core/Core.csproj", "Core/"]
RUN dotnet restore "./Jobs/Jobs.csproj"
COPY src/ .
RUN dotnet publish "./Jobs/Jobs.csproj" -c $BUILD_CONFIGURATION -o /app

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated9.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated9.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
  AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=build ["/app", "/home/site/wwwroot"]
