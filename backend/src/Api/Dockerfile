# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY *.props .
COPY ["src/Api/Api.csproj", "Api/"]
COPY ["src/Database/Database.csproj", "Database/"]
COPY ["src/ImageHosting/ImageHosting.csproj", "ImageHosting/"]
COPY ["src/ServiceDefaults/ServiceDefaults.csproj", "ServiceDefaults/"]
COPY ["src/Storage/Storage.csproj", "Storage/"]
COPY ["src/Core/Core.csproj", "Core/"]
RUN dotnet restore "./Api/Api.csproj" --locked-mode
COPY src/ .
RUN dotnet publish "/src/Api/Api.csproj" -c $BUILD_CONFIGURATION -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "RateMyPet.Api.dll"]
