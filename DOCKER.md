# Dockerización de Nexus

Esta solución se dockeriza en 2 contenedores orquestados con `docker-compose`. La base de
datos **no se dockeriza**: se asume que ya tienes un PostgreSQL administrado en la nube
(Azure Database for PostgreSQL, AWS RDS, Supabase, Neon, Render, etc.) y el backend se
conecta a él por red.

| Servicio             | Contenido                                | Puerto host |
|-----------------------|-------------------------------------------|-------------|
| `nexus.api`           | Backend (.NET 8, ASP.NET Core)            | `5100`      |
| `nexus.presentation`  | Frontend (Blazor WASM servido por Nginx)  | `5250`      |

```
Nexus/
├── docker-compose.yml              # backend + frontend (BD en la nube)
├── docker-compose.local-db.yml     # OPCIONAL: agrega un Postgres local
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
- Una base de datos PostgreSQL accesible desde donde corras estos contenedores (nube o local), con la cadena de conexión a mano.
- .NET SDK 8 instalado localmente **solo para el paso 1** (generar la migración inicial), a menos que ya la tengas generada en el repo.

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
volver a hacer esto salvo que cambies el modelo de datos — en ese caso, generas una
migración nueva con `dotnet ef migrations add <Nombre>` y la subes al repo igual que
cualquier otro cambio de código.

## Paso 2 — Configurar variables de entorno

```bash
cp .env.example .env
```

Edita `.env` y completa al menos:
- **`NEXUS_DB_CONNECTION_STRING`** — la cadena de conexión completa a tu base de datos en la nube. En `.env.example` hay ejemplos concretos para Azure, AWS RDS, Supabase y Neon. La mayoría de proveedores administrados **exigen SSL** (`SSL Mode=Require`); si lo omites, la conexión probablemente falle.
- **`JWT_SECRET`** (genera uno con `openssl rand -base64 48`, por ejemplo).
- **`API_BASE_URL`** (en local, déjalo en `http://localhost:5100`; en un despliegue real, pon la URL pública de tu API).

`.env` está en `.gitignore` — nunca se sube al repositorio.

### ¿Tu base de datos en la nube no es accesible desde donde corres Docker?

Si tu proveedor restringe el acceso por IP (whitelist), asegúrate de agregar la IP pública
de la máquina/servidor donde corres `docker compose` a la lista de conexiones permitidas de
tu proveedor de PostgreSQL. Esto es independiente de Docker — es una regla de firewall del
lado del proveedor de la base de datos.

## Paso 3 — Construir y levantar los contenedores

```bash
docker compose build
docker compose up -d
```

Con `RUN_MIGRATIONS_ON_STARTUP=true` (valor por defecto en `.env.example`), el contenedor
`nexus.api` aplica las migraciones pendientes automáticamente en cada arranque contra tu
base de datos en la nube — no necesitas correr `dotnet ef database update` a mano.

## Paso 4 — Verificar que todo funciona

- **Backend / Swagger**: http://localhost:5100/swagger (Swagger solo se expone si `ASPNETCORE_ENVIRONMENT=Development`; en `Production` no aparece, por diseño).
- **Frontend**: http://localhost:5250 → debería redirigirte a `/login`.
- **Logs**:
  ```bash
  docker compose logs -f nexus.api
  docker compose logs -f nexus.presentation
  ```

## ¿Necesitas un Postgres local de todos modos? (pruebas/desarrollo)

Si en algún momento quieres probar contra un Postgres local (por ejemplo, para no tocar
datos reales de la nube mientras desarrollas), hay un archivo de superposición **opcional**
que agrega ese servicio sin modificar el `docker-compose.yml` principal:

```bash
docker compose -f docker-compose.yml -f docker-compose.local-db.yml up -d --build
```

Esto levanta un contenedor `postgres` adicional y hace que `nexus.api` se conecte a él en
vez de a `NEXUS_DB_CONNECTION_STRING`. Para volver a apuntar a la nube, simplemente vuelve
a usar `docker compose up -d` (sin el `-f` adicional).

## Comandos útiles

```bash
# Reconstruir solo un servicio tras cambiar código
docker compose build nexus.api
docker compose up -d nexus.api

# Ver el estado de los contenedores
docker compose ps

# Apagar todo
docker compose down
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

La conexión a la base de datos en la nube sí va cifrada (`SSL Mode=Require` en la cadena de
conexión), independientemente de esto.

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
  de `nexus.api`), apuntando a la misma `NEXUS_DB_CONNECTION_STRING`.

### 6. Si dejas `NEXUS_DB_CONNECTION_STRING` vacío
El `docker-compose.yml` principal no falla al analizar el archivo si `NEXUS_DB_CONNECTION_STRING`
no está definida (a propósito, para que el overlay `docker-compose.local-db.yml` pueda
sobrescribirla sin necesitar un valor "dummy" en `.env`). Si la dejas vacía **y no usas** el
overlay local, el contenedor `nexus.api` arrancará pero fallará al conectar a la base de
datos — revisa `docker compose logs nexus.api` para ver el error exacto de Npgsql.

## Próximos pasos sugeridos

- Publicar las imágenes en un registro (Docker Hub, GitHub Container Registry, ACR, ECR) para
  desplegar en un servidor/orquestador real en vez de solo `docker compose` local.
- Agregar un `docker-compose.override.yml` o perfiles separados para desarrollo (con hot-reload
  vía `dotnet watch` montando el código como volumen) vs. producción (las imágenes ya construidas).
- Healthchecks HTTP para `nexus.api` y `nexus.presentation` en el `docker-compose.yml` (hoy
  ninguno tiene, ya que no depende de un Postgres local con healthcheck propio), para que
  `depends_on` espere a que el backend esté realmente respondiendo.
