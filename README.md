# Nexus — Backend (.NET 8, Clean Architecture)

Backend de la aplicación **Nexus**, construido con **Clean Architecture** (Domain,
Application, Infrastructure, Api), **Entity Framework Core (Code First)** contra
**NexusDB** en **PostgreSQL** (proveedor [Npgsql](https://www.npgsql.org/efcore/)),
autenticación con **JWT**, contraseñas con **hashing seguro (PBKDF2)**,
documentación con **Swagger** y **middleware global de excepciones**.

## Estructura del proyecto

```
Nexus.sln
src/
  Nexus.Domain/          -> Entidades del dominio (User, BaseEntity)
  Nexus.Application/     -> DTOs, interfaces, servicios de aplicación, excepciones
  Nexus.Infrastructure/  -> EF Core (NexusDbContext), repositorios, JWT, hashing, DI
  Nexus.Api/              -> Controllers, Program.cs, Swagger, middleware, appsettings
```

Reglas de dependencia (Clean Architecture):
`Api -> Infrastructure -> Application -> Domain` (Application y Domain no dependen de nada externo).

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (instancia local, Docker o un servicio administrado)
- (Opcional) [dotnet-ef CLI](https://learn.microsoft.com/ef/core/cli/dotnet):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Levantar PostgreSQL rápidamente con Docker (opcional)

```bash
docker run --name nexus-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=CAMBIA_ESTA_PASSWORD \
  -e POSTGRES_DB=NexusDB \
  -p 5432:5432 \
  -d postgres:16
```

## 1. Restaurar paquetes

```bash
cd Nexus
dotnet restore
```

## 2. Configurar la conexión y el secreto JWT

Edita `src/Nexus.Api/appsettings.json` (o mejor, usa **User Secrets** en desarrollo
para no versionar credenciales):

```bash
cd src/Nexus.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:NexusDB" "Host=localhost;Port=5432;Database=NexusDB;Username=postgres;Password=TU_PASSWORD"
dotnet user-secrets set "JwtSettings:Secret" "una-llave-secreta-larga-y-aleatoria-de-al-menos-32-caracteres"
```

> Si tu proveedor de PostgreSQL requiere SSL (por ejemplo, en un servicio en la
> nube), agrega `;SSL Mode=Require;Trust Server Certificate=true` al final de la
> cadena de conexión.

⚠️ **Importante**: cambia el valor de `JwtSettings:Secret` antes de desplegar a
producción. El valor incluido en `appsettings.json` es solo un placeholder.

## 3. Crear la migración inicial y la base de datos NexusDB

Desde la raíz de la solución (`Nexus/`):

```bash
dotnet ef migrations add InitialCreate \
  --project src/Nexus.Infrastructure \
  --startup-project src/Nexus.Api

dotnet ef database update \
  --project src/Nexus.Infrastructure \
  --startup-project src/Nexus.Api
```

Esto crea la base de datos `NexusDB` con la tabla `Users` (Code First).

## 3.1 Migración del módulo de Integraciones

Se agregaron 5 entidades nuevas (`IntgEmpresas`, `IntgConectores`, `IntgIntegraciones`,
`IntgIntegracionConectores`, `IntgEmpresaIntegracionConectores`) con sus relaciones,
índices únicos y llaves foráneas ya configuradas en `Nexus.Infrastructure/Persistence/Configurations/Integraciones`.

Genera y aplica la migración correspondiente:

```bash
dotnet ef migrations add AddIntegracionesModule \
  --project src/Nexus.Infrastructure \
  --startup-project src/Nexus.Api

dotnet ef database update \
  --project src/Nexus.Infrastructure \
  --startup-project src/Nexus.Api
```

**Notas de diseño:**
- Todas las llaves primarias son `BIGINT` autoincremental (identity), no `Guid`.
- `ConfiguracionAdicionalJSON` se mapea como `jsonb` (nativo de PostgreSQL) para
  permitir indexado y consultas sobre el contenido del JSON si se requiere a futuro.
- Las FK usan `DeleteBehavior.Restrict`: no se puede borrar una Empresa, Conector
  o Integración si tiene registros dependientes, evitando borrados en cascada
  accidentales sobre datos maestros/de configuración.
- Se agregaron índices únicos de negocio:
  - `IntgEmpresas`: (`TipoIdentificacion`, `NumeroIdentificacion`)
  - `IntgConectores`: `Nombre`
  - `IntgIntegraciones`: `CodigoAccion`
  - `IntgIntegracionConectores`: (`IntegracionId`, `ConectorId`)
  - `IntgEmpresaIntegracionConectores`: (`EmpresaId`, `IntegracionConectorId`)

## 4. Ejecutar la API

```bash
dotnet run --project src/Nexus.Api
```

Swagger quedará disponible (en entorno Development) en:

```
https://localhost:5101/swagger
http://localhost:5100/swagger
```

## Endpoints disponibles

| Método | Ruta                  | Descripción                                  | Autenticación |
|--------|-----------------------|-----------------------------------------------|----------------|
| POST   | `/api/auth/register`  | Registra un nuevo usuario                     | Anónimo        |
| POST   | `/api/auth/login`     | Autentica y devuelve un token JWT             | Anónimo        |
| GET    | `/api/auth/me`        | Devuelve los claims del usuario autenticado   | JWT (Bearer)   |

### Ejemplo — Registro

```json
POST /api/auth/register
{
  "fullName": "Ana Torres",
  "email": "ana.torres@example.com",
  "password": "ClaveSegura123!",
  "confirmPassword": "ClaveSegura123!"
}
```

### Ejemplo — Login

```json
POST /api/auth/login
{
  "email": "ana.torres@example.com",
  "password": "ClaveSegura123!"
}
```

La respuesta incluye el token JWT:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-08-19T20:00:00Z",
  "user": {
    "id": "b1a2c3d4-...",
    "fullName": "Ana Torres",
    "email": "ana.torres@example.com",
    "role": "User"
  }
}
```

Para consumir un endpoint privado, agrega el header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

En Swagger, usa el botón **Authorize** e ingresa `Bearer {tu_token}`.

## Seguridad implementada

- **Contraseñas**: hashing con PBKDF2 (SHA-256, 100.000 iteraciones, salt aleatorio
  de 128 bits por usuario). Nunca se almacena la contraseña en texto plano.
  Comparación en tiempo constante para evitar *timing attacks*.
- **JWT**: firmado con HMAC-SHA256, incluye claims de identidad y rol, con
  expiración configurable (`JwtSettings:ExpiryMinutes`).
- **Manejo global de excepciones**: `ExceptionHandlingMiddleware` captura toda
  excepción no controlada y devuelve una respuesta JSON consistente, sin exponer
  detalles internos fuera de `Development`.
- **Validación de entrada**: DataAnnotations en los DTOs de registro/login.

## Reconstrucción completa del backend (todos los módulos)

El backend fue reconstruido en su totalidad a partir de la documentación Swagger (`swagger.json`)
y el esquema de base de datos (`NexusDB.sql`) que ya tenías. Además del módulo de Auth, ahora incluye:

- **Empresas** (`/api/v1/Empresas`)
- **Conectores** (`/api/v1/Conectores`)
- **Integraciones** (`/api/v1/Integraciones`)
- **IntegracionConectores** (`/api/v1/IntegracionConectores`)
- **ConfiguracionEnrutamiento** (`/api/v1/ConfiguracionEnrutamiento`), incluyendo el endpoint
  `GET /resolver` que arma en caliente la URL completa (`UrlBase` + `RutaEndpoint`) para una
  empresa + código de acción.
- **Aplicaciones** (`/api/v1/Aplicaciones`)
- **AplicacionIntegraciones**, **AplicacionEmpresas**, **AplicacionConectores**
- **EmpresaConectores** (relación 1 a 1 entre Empresa y Conector)

Los 60 endpoints del `swagger.json` provisto están implementados uno a uno, respetando incluso
sus particularidades:
- `POST /AplicacionIntegraciones` devuelve **201 sin cuerpo** (esa relación no tiene un
  identificador propio expuesto en el contrato).
- `PATCH /AplicacionConectores/{id}/estado` recibe un **booleano crudo** en el body
  (`true`/`false`), a diferencia de todos los demás endpoints de estado que usan `{ "activo": bool }`.
- `GET /EmpresaConectores/empresa/{empresaId}` devuelve un **objeto único** (no un arreglo):
  la relación Empresa-Conector es 1 a 1.

### ⚠️ Dos discrepancias encontradas entre `NexusDB.sql` y `swagger.json`

1. **`IntgEmpresaIntegracionConectores` no aparece en el `.sql` provisto**, pero el swagger sí
   define completamente sus endpoints (`ConfiguracionEnrutamiento`). Se reconstruyó la entidad y
   su migración la creará si no existe. Si en tu base de datos real esa tabla ya existe con otro
   nombre o columnas distintas, avísame para ajustarlo antes de migrar en producción.
2. **`IntgAplicacionConector`** en el `.sql` solo tiene columnas `UsuarioErp` y `PasswordErp`,
   pero el contrato de la API (`CrearAplicacionConectorDto` / `ActualizarAplicacionConectorDto`)
   también incluye `UrlBasePersonalizada`, `ApiKey` y `TokenBearer`. Se agregaron esas columnas
   a la entidad para que la migración las incluya, dejando la tabla alineada con lo que el
   frontend ya consume.

Con estos cambios, corre una migración incremental (no necesitas recrear la base de datos):

```bash
dotnet ef migrations add ReconstruccionCompleta \
  --project src/Nexus.Infrastructure \
  --startup-project src/Nexus.Api

dotnet ef database update \
  --project src/Nexus.Infrastructure \
  --startup-project src/Nexus.Api
```

Si tu base de datos real ya tiene todas estas tablas creadas manualmente (fuera de EF), en vez de
`database update` puedes usar `dotnet ef migrations add ReconstruccionCompleta --json` para
inspeccionar el script y aplicarlo selectivamente, o marcar la migración como ya aplicada con
`dotnet ef database update <NombreMigraciónAnterior>` seguido de un `INSERT` manual en
`__EFMigrationsHistory` si prefieres no tocar las tablas existentes.

## Próximos pasos sugeridos

- Agregar roles y políticas de autorización más granulares (`[Authorize(Roles = "Admin")]`).
- Agregar *refresh tokens* para renovar la sesión sin reautenticar.
- Agregar pruebas unitarias (xUnit) para `AuthService` y `JwtService`.
- Agregar `FluentValidation` si las reglas de validación crecen en complejidad.
- Configurar `Serilog` para logging estructurado.
