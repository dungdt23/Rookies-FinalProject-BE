#Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

#Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

#COPY csproj files from all layers in clean architecture
COPY ["AssetManagement.Api/AssetManagement.Api.csproj", "AssetManagement.Api/"]
COPY ["AssetManagement.Application/AssetManagement.Application.csproj", "AssetManagement.Application/"]
COPY ["AssetManagement.Domain/AssetManagement.Domain.csproj", "AssetManagement.Domain/"]
COPY ["AssetManagement.Infrastructure/AssetManagement.Infrastructure.csproj", "AssetManagement.Infrastructure/"]

#Restore all dependencies
RUN dotnet restore "./AssetManagement.Api/AssetManagement.Api.csproj"

# Copy the rest of the source code
COPY . .

# Set working directory to API layer
WORKDIR "/src/AssetManagement.Api"

# Build the solution
RUN dotnet build "AssetManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AssetManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AssetManagement.Api.dll"]