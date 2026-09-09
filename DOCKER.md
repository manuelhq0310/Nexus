# Dockerización de Nexus

Esta solución se dockeriza en 3 contenedores orquestados con `docker-compose`:

| Servicio            | Contenido                                | Puerto host |
|---------------------|-------------------------------------------|-------------|
| `postgres`          | PostgreSQL 16                             | `5432`      |
| `nexus.api`          | Backend (.NET 8, ASP.NET Core)            | `5100`      |
| `nexus.presentation` | Frontend (Blazor WASM servido por Nginx)  | `5250`      |

```
Nexus/
├── docker-compose.yml
├── .env.example
├── .dockerignore
└── src/
    ├── Nexus.Api/
    │   └── Dockerfile
    └── Nexus.Presentation/
        ├── Dockerfile
        ├── nginx.conf
        ├── docker-entrypoint.sh
        └── wwwroot/appsettings.template.json
```

## Requisitos previos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (incluye Compose) o Docker Engine + plugin `docker compose` en Linux.
- Nada de .NET SDK instalado localmente es estrictamente necesario para *correr* los contenedores — pero **sí lo necesitas para el paso 1** (generar la migración inicial), a menos que ya la tengas generada.

## Paso 1 — Generar la migración inicial de EF Core (una sola vez)

⚠️ **Importante**: si tu proyecto todavía no tiene ninguna migración de EF Core generada
(carpeta `src/Nexus.Infrastructure/Persistence/Migrations` vacía o inexistente), el backend
arrancará sin errores pero **la base de datos quedará vacía** — `Database.Migrate()` no crea
tablas de la nada, solo aplica migraciones que ya existen como código. Genera la migración
inicial con el SDK de .NET 8 instalado en tu máquina (no dentro de Docker):

```bash
dotnet tool install --global dotnet-ef   # si no lo tienes
cd Nexus
dotnet ef migrations add InitialCreate \
  --project src/Nexus.Infrastructure \
  --startup-project src/Nexus.Api
```

Esto genera los archivos de migración como código C# dentro del repo. Ya no necesitas
volver a hacer esto salvo que cambies el modelo de datos (agregar una entidad, una
propiedad, etc.) — en ese caso, generas una migración nueva con `dotnet ef migrations add <Nombre>`
y la subes al repo igual que cualquier otro cambio de código.

## Paso 2 — Configurar variables de entorno

```bash
cp .env.example .env
```

Edita `.env` y completa al menos:
- `POSTGRES_PASSWORD`
- `JWT_SECRET` (genera uno con `openssl rand -base64 48`, por ejemplo)
- `API_BASE_URL` (en local, déjalo en `http://localhost:5100`; en un despliegue real, pon la URL pública de tu API)

`.env` está en `.gitignore` — nunca se sube al repositorio.

## Paso 3 — Construir y levantar los contenedores

```bash
docker compose build
docker compose up -d
```

Con `RUN_MIGRATIONS_ON_STARTUP=true` (valor por defecto en `.env.example`), el contenedor
`nexus.api` aplica las migraciones pendientes automáticamente en cada arranque — no necesitas
correr `dotnet ef database update` a mano contra el contenedor.

## Paso 4 — Verificar que todo funciona

- **Backend / Swagger**: http://localhost:5100/swagger (Swagger solo se expone si `ASPNETCORE_ENVIRONMENT=Development`; en `Production` no aparece, por diseño).
- **Frontend**: http://localhost:5250 → debería redirigirte a `/login`.
- **Logs**:
  ```bash
  docker compose logs -f nexus.api
  docker compose logs -f nexus.presentation
  ```

## Comandos útiles

```bash
# Reconstruir solo un servicio tras cambiar código
docker compose build nexus.api
docker compose up -d nexus.api

# Ver el estado de los contenedores
docker compose ps

# Apagar todo (conserva los datos de Postgres, guardados en el volumen nexus_postgres_data)
docker compose down

# Apagar todo y BORRAR también los datos de Postgres
docker compose down -v

# Entrar a la base de datos
docker compose exec postgres psql -U postgres -d NexusDB
```

## Decisiones de diseño / cosas a tener en cuenta

### 1. La URL del API en el frontend es configurable en tiempo de ejecución, no en build
El frontend es un SPA estático (Blazor WebAssembly): el navegador del usuario, no el
servidor, es quien hace las llamadas HTTP al backend. Por eso `wwwroot/appsettings.json`
**no puede** apuntar al nombre interno de red de Docker (`nexus.api`), que solo es
resoluble *entre* contenedores, no desde tu navegador.

Para no tener que reconstruir la imagen del frontend cada vez que cambia la URL pública
del API, se usa una plantilla (`wwwroot/appsettings.template.json`) con el placeholder
`__API_BASE_URL__`. El script `docker-entrypoint.sh` la sustituye por el valor de la
variable de entorno `API_BASE_URL` **al arrancar el contenedor**, generando el
`appsettings.json` real justo antes de iniciar Nginx. La misma imagen sirve para local,
staging y producción — solo cambia la variable de entorno.

### 2. CORS
El backend tiene una política CORS abierta (`AllowAnyOrigin`) heredada del desarrollo
local. Funciona tal cual con este `docker-compose.yml`, pero **para producción real** te
recomiendo restringirla al dominio exacto del frontend en `Nexus.Api/Program.cs`:

```csharp
policy.WithOrigins("https://tu-dominio-real.com")
      .AllowAnyMethod()
      .AllowAnyHeader();
```

### 3. TLS / HTTPS
Ninguno de los dos Dockerfiles configura HTTPS directamente: ambos escuchan en HTTP plano
dentro de la red de contenedores. Esto es intencional — el patrón estándar es terminar TLS
en una capa delante de los contenedores (Nginx/Traefik como reverse proxy, un load balancer
gestionado, Cloudflare, etc.) y que el tráfico *interno* hacia `nexus.api`/`nexus.presentation`
viaje en HTTP plano dentro de tu red privada/Docker. Si despliegas esto directamente expuesto
a internet sin un proxy TLS delante, los tokens JWT viajarían sin cifrar.

### 4. Compresión Brotli/Gzip del build de Blazor
Se publicó con `-p:BlazorEnableCompression=false` para simplificar el `nginx.conf` inicial
(Nginx sirve los archivos sin comprimir; el `gzip on` de `nginx.conf` comprime al vuelo,
que es menos eficiente que servir los `.br` precomprimidos por el propio build de .NET).
Si más adelante quieres optimizar la transferencia:
1. Quita `-p:BlazorEnableCompression=false` del `Dockerfile` del frontend.
2. Agrega el módulo `brotli` a la imagen de Nginx (la imagen `nginx:alpine` no lo trae por
   defecto; hay imágenes de terceros como `fholzer/nginx-brotli` que sí, o se compila el módulo).
3. Configura `nginx.conf` para servir el archivo `.br` correspondiente cuando el navegador
   lo soporte (`gzip_static`/`brotli_static`).

### 5. Migración automática en producción
`RUN_MIGRATIONS_ON_STARTUP=true` es cómodo para desarrollo/staging, pero en un entorno de
producción con múltiples réplicas del backend corriendo en paralelo, aplicar migraciones
desde cada instancia que arranca puede causar condiciones de carrera. Para producción,
considera:
- Desactivar `RUN_MIGRATIONS_ON_STARTUP` (ponerlo en `false`), y
- Correr las migraciones como un paso separado del pipeline de despliegue (un job/contenedor
  que corre una sola vez, con `dotnet ef database update`, antes de desplegar la nueva versión
  de `nexus.api`).

## Próximos pasos sugeridos

- Publicar las imágenes en un registro (Docker Hub, GitHub Container Registry, ACR, ECR) para
  desplegar en un servidor/orquestador real en vez de solo `docker compose` local.
- Agregar un `docker-compose.override.yml` o perfiles separados para desarrollo (con hot-reload
  vía `dotnet watch` montando el código como volumen) vs. producción (las imágenes ya construidas).
- Healthchecks HTTP para `nexus.api` y `nexus.presentation` en el `docker-compose.yml` (hoy solo
  `postgres` tiene healthcheck), para que `depends_on` espere a que el backend esté realmente
  respondiendo, no solo a que el contenedor haya arrancado.
