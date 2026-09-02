# Nexus.Presentation — Frontend (Blazor WebAssembly .NET 10 + MudBlazor)

Frontend SPA que consume la API de Nexus (`Nexus.Api`) para: iniciar sesión,
administrar Empresas, Conectores e Integraciones, y configurar las relaciones
entre ellas mediante un asistente paso a paso (stepper).

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- El backend `Nexus.Api` corriendo (ver README de la raíz de la solución)

## 1. Configurar la URL de la API

Edita `wwwroot/appsettings.json`:

```json
{
  "ApiBaseUrl": "https://localhost:5101"
}
```

Debe apuntar al mismo host/puerto donde corre `Nexus.Api`. Si tu backend usa
otro puerto (revisa `Properties/launchSettings.json` en `Nexus.Api`), ajústalo aquí.

> **CORS**: la política configurada en `Nexus.Api/Program.cs` (`AllowAnyOrigin`)
> ya permite llamadas desde este frontend sin configuración adicional. Si la
> restringes en el futuro, agrega el origen de este proyecto (ej. `http://localhost:5250`).

## 2. Restaurar y ejecutar

Desde la raíz de la solución (`Nexus/`):

```bash
dotnet restore
dotnet run --project src/Nexus.Presentation
```

Por defecto se sirve en `http://localhost:5250` (ver `Properties/launchSettings.json`
que se genera al restaurar/ejecutar por primera vez, o defínelo tú mismo si no existe).

Si prefieres el hot-reload de desarrollo:

```bash
dotnet watch --project src/Nexus.Presentation
```

## 3. Primer uso

1. Abre la app → te redirige a `/login`.
2. Si no tienes usuario, entra a **"Regístrate aquí"** y crea uno (usa el mismo
   endpoint `/api/Auth/register` del backend).
3. Inicia sesión. El token JWT se guarda en `localStorage` del navegador y se
   adjunta automáticamente a cada llamada a la API (`JwtAuthorizationMessageHandler`).
4. Sigue el flujo sugerido en el panel general:
   1. **Integraciones** → crea al menos una (ej. `REGISTRAR_ANTICIPO`).
   2. **Conectores** → crea al menos uno (ej. "Conector SAP ECC").
   3. **Empresas → Nueva empresa** → asistente de 3 pasos:
      - Paso 1: datos de la empresa.
      - Paso 2: seleccionar la integración a asignar.
      - Paso 3: seleccionar (o crear al vuelo) el conector que resuelve esa
        integración, más credenciales opcionales de la empresa.

Desde el detalle de una empresa (`/empresas/{id}`) puedes agregarle más
integraciones en cualquier momento con el mismo asistente (2 pasos, ya que la
empresa ya existe).

## Estructura del proyecto

```
Program.cs                  -> DI, HttpClient con JWT, MudBlazor, autenticación
App.razor                   -> Enrutamiento + guard de autenticación
Layout/                     -> MainLayout (AppBar + Drawer) y NavMenu
Models/                     -> DTOs que reflejan los contratos del swagger.json
Services/
  LocalStorageService.cs    -> Wrapper de localStorage vía JS interop
  Auth/                     -> AuthService, CustomAuthStateProvider, JwtAuthorizationMessageHandler
  Api/                      -> Un servicio HTTP tipado por módulo del backend
Pages/
  Login.razor, Register.razor
  Home.razor                -> Panel general (dashboard)
  Empresas/                 -> Listado, asistente de creación (3 pasos),
                                detalle + "agregar relación" (2 pasos), diálogos de edición
  Conectores/, Integraciones/ -> Listado + diálogo de creación/edición (forma simple)
Shared/
  StepHeader.razor          -> Indicador visual de stepper (reutilizado en los asistentes)
  RedirectToLogin.razor     -> Redirección cuando una ruta protegida no está autenticada
```

## Notas de diseño

- **Sin dependencias extra para auth/localStorage**: se implementó un wrapper
  propio sobre `localStorage` (`LocalStorageService`) en vez de agregar un
  paquete de terceros, para mantener la superficie de dependencias mínima.
- **El stepper es un componente propio** (`StepHeader.razor`, con CSS en
  `wwwroot/css/app.css`) en lugar del `MudStepper` nativo de MudBlazor, para
  tener control total sobre las llamadas asíncronas a la API entre cada paso
  (crear la empresa al pasar del paso 1 al 2, cargar los conectores disponibles
  al pasar del paso 2 al 3, etc.) sin depender de una API de terceros que no
  pudo verificarse por compilación en este entorno.
- **Relación Integración↔Conector "al vuelo"**: en el paso 3 del asistente,
  si la integración elegida aún no tiene ningún conector asociado, el asistente
  permite crear esa relación (`POST /api/v1/IntegracionConectores`) sin salir
  del flujo, y luego crea la relación final Empresa↔Integración↔Conector
  (`POST /api/v1/ConfiguracionEnrutamiento`).

## ⚠️ Importante: no se pudo compilar en este entorno

Este proyecto se generó sin acceso a un SDK de .NET ni a NuGet en el entorno
de trabajo, por lo que **no fue posible ejecutar `dotnet build` para
verificarlo**. Se revisó cuidadosamente el código (usings, tipos, nombres de
parámetros de MudBlazor) pero, al restaurar por primera vez, revisa:

- Que la versión de `MudBlazor` (`8.*` en el `.csproj`) resuelva correctamente;
  si tu feed tiene una versión distinta, ajusta el número de versión.
- Que los componentes de MudBlazor usados (`MudTable`, `MudSelect`, `MudDialog`,
  `MudTextField`, etc.) coincidan con la versión resuelta — la API pública de
  estos componentes es estable entre versiones recientes, pero si `dotnet build`
  marca algún parámetro como inexistente, probablemente cambió de nombre en tu
  versión específica.

## Próximos pasos sugeridos

- Manejo de expiración de token con refresh automático o aviso al usuario.
- Paginación server-side en los listados si el volumen de datos crece.
- Página dedicada para gestionar directamente las relaciones Integración↔Conector
  (hoy solo se gestionan implícitamente desde el asistente de Empresas).
- Tests con bUnit para los componentes críticos (asistente de creación, AuthService).
