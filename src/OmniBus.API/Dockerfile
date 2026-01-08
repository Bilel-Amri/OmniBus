# OmniBus API - ASP.NET 8 (Multi-stage optimized)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Add non-root user for security
RUN groupadd -r omnibus && useradd -r -g omnibus omnibus
RUN chown -R omnibus:omnibus /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only project files first for better caching
COPY ["src/OmniBus.API/OmniBus.API.csproj", "OmniBus.API/"]
COPY ["src/OmniBus.Application/OmniBus.Application.csproj", "OmniBus.Application/"]
COPY ["src/OmniBus.Domain/OmniBus.Domain.csproj", "OmniBus.Domain/"]
COPY ["src/OmniBus.Infrastructure/OmniBus.Infrastructure.csproj", "OmniBus.Infrastructure/"]
COPY ["global.json", "./"]

# Restore dependencies (cached if project files don't change)
RUN dotnet restore "OmniBus.API/OmniBus.API.csproj"

# Copy everything else and build
COPY src/ .
RUN dotnet build "OmniBus.API/OmniBus.API.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "OmniBus.API/OmniBus.API.csproj" -c Release -o /app/publish \
    --no-restore --no-build \
    /p:UseAppHost=false \
    /p:DebugType=None \
    /p:DebugSymbols=false

FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Switch to non-root user
USER omnibus

# Add health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

ENTRYPOINT ["dotnet", "OmniBus.API.dll"]
