# syntax=docker/dockerfile:1

# ==========================================================================
# Etapa 1: build + publish (Blazor WebAssembly)
# ==========================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# El workload wasm-tools no viene preinstalado en la imagen del SDK.
RUN dotnet workload install wasm-tools

COPY src/Nexus.Presentation/Nexus.Presentation.csproj src/Nexus.Presentation/
RUN dotnet restore src/Nexus.Presentation/Nexus.Presentation.csproj

COPY src/Nexus.Presentation/ src/Nexus.Presentation/

# BlazorEnableCompression=false: evita depender de una configuración de Nginx
# que sirva los .br/.gz precomprimidos con el Content-Encoding correcto.
# Es una simplificación deliberada para tener el contenedor funcionando rápido;
# ver DOCKER.md para cómo reactivar la compresión si se necesita optimizar transferencia.
RUN dotnet publish src/Nexus.Presentation/Nexus.Presentation.csproj \
    -c Release \
    -o /app/publish \
    -p:BlazorEnableCompression=false \
    --no-restore

# ==========================================================================
# Etapa 2: runtime (Nginx sirviendo los archivos estáticos)
# ==========================================================================
FROM nginx:1.27-alpine AS runtime

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY src/Nexus.Presentation/nginx.conf /etc/nginx/conf.d/default.conf
COPY src/Nexus.Presentation/docker-entrypoint.sh /docker-entrypoint.sh

RUN chmod +x /docker-entrypoint.sh

EXPOSE 80
ENTRYPOINT ["/docker-entrypoint.sh"]
