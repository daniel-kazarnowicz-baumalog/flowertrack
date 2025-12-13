# =============================================================================
# FLOWerTRACK Backend - Dockerfile
# =============================================================================
# Multi-stage build for .NET 10.0 ASP.NET Core API
# Optimized for Render.com deployment
# =============================================================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy solution and project files first (for better caching)
COPY ["Flowertrack.sln", "./"]
COPY ["Core/Flowertrack.Domain/Flowertrack.Domain.csproj", "Core/Flowertrack.Domain/"]
COPY ["Core/Flowertrack.Application/Flowertrack.Application.csproj", "Core/Flowertrack.Application/"]
COPY ["Infrastructure/Flowertrack.Infrastructure/Flowertrack.Infrastructure.csproj", "Infrastructure/Flowertrack.Infrastructure/"]
COPY ["Presentation/Flowertrack.Api/Flowertrack.Api.csproj", "Presentation/Flowertrack.Api/"]
COPY ["Presentation/Flowertrack.Contracts/Flowertrack.Contracts.csproj", "Presentation/Flowertrack.Contracts/"]

# Restore dependencies
RUN dotnet restore "Presentation/Flowertrack.Api/Flowertrack.Api.csproj"

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/Presentation/Flowertrack.Api"
RUN dotnet publish "Flowertrack.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Expose port (Render uses PORT env variable)
EXPOSE 8080

# Health check (using wget instead of curl for minimal images)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Run the application
ENTRYPOINT ["dotnet", "Flowertrack.Api.dll"]
