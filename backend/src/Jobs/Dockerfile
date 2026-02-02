# Jobs Functions App Dockerfile
# This needs to be run from the backend directory (build context)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY *.props .
COPY ["src/Jobs/Jobs.csproj", "Jobs/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/Core/Core.csproj", "Core/"]
RUN dotnet restore "./Jobs/Jobs.csproj"
COPY src/ .
RUN dotnet publish "./Jobs/Jobs.csproj" -c $BUILD_CONFIGURATION -o /app

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0
COPY --from=build ["/app", "/home/site/wwwroot"]
ENV AzureWebJobsScriptRoot=/home/site/wwwroot
ENV AzureFunctionsJobHost__Logging__Console__IsEnabled=true
LABEL org.opencontainers.image.source=https://github.com/frasermclean/ratemypet
