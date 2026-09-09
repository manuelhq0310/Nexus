# syntax=docker/dockerfile:1

# ==========================================================================
# Etapa 1: build + publish
# ==========================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos primero solo los .csproj para aprovechar el cache de capas de Docker:
# mientras no cambien las dependencias, `dotnet restore` no se vuelve a ejecutar
# aunque cambie el código fuente.
COPY src/Nexus.Domain/Nexus.Domain.csproj src/Nexus.Domain/
COPY src/Nexus.Application/Nexus.Application.csproj src/Nexus.Application/
COPY src/Nexus.Infrastructure/Nexus.Infrastructure.csproj src/Nexus.Infrastructure/
COPY src/Nexus.Api/Nexus.Api.csproj src/Nexus.Api/
RUN dotnet restore src/Nexus.Api/Nexus.Api.csproj

# Ahora sí copiamos el código fuente completo de los 4 proyectos y publicamos.
COPY src/Nexus.Domain/ src/Nexus.Domain/
COPY src/Nexus.Application/ src/Nexus.Application/
COPY src/Nexus.Infrastructure/ src/Nexus.Infrastructure/
COPY src/Nexus.Api/ src/Nexus.Api/

RUN dotnet publish src/Nexus.Api/Nexus.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ==========================================================================
# Etapa 2: runtime (imagen final, mucho más liviana que el SDK)
# ==========================================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Usuario sin privilegios (buena práctica de seguridad en contenedores).
RUN useradd --uid 1000 --create-home nexus
USER nexus

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080

ENTRYPOINT ["dotnet", "Nexus.Api.dll"]
