using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Http;
using MudBlazor.Services;
using Nexus.Presentation;
using Nexus.Presentation.Services;
using Nexus.Presentation.Services.Api;
using Nexus.Presentation.Services.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// La URL base de la API se lee de wwwroot/appsettings.json ("ApiBaseUrl").
// Blazor WASM carga ese archivo automáticamente dentro de builder.Configuration.
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("Falta configurar 'ApiBaseUrl' en wwwroot/appsettings.json");

// Validación temprana y explícita: un valor mal configurado (por ejemplo, el nombre
// interno de un servicio de Docker Compose como "nexus_api", sin esquema ni puerto)
// hace que "new Uri(apiBaseUrl)" falle más abajo con un UriFormatException genérico,
// en medio de un ciclo de render de Blazor — un error muy difícil de diagnosticar
// desde la consola del navegador. Se valida aquí, con un mensaje que dice exactamente
// qué está mal y cómo corregirlo.
if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var parsedApiBaseUrl) ||
    (parsedApiBaseUrl.Scheme != Uri.UriSchemeHttp && parsedApiBaseUrl.Scheme != Uri.UriSchemeHttps))
{
    throw new InvalidOperationException(
        $"'ApiBaseUrl' = \"{apiBaseUrl}\" no es una URL http(s) absoluta válida. " +
        "Debe incluir el esquema y ser una dirección alcanzable desde el NAVEGADOR del usuario " +
        "(NO el nombre interno de un servicio de Docker Compose, como \"nexus_api\", que solo " +
        "resuelven los contenedores entre sí). Ejemplo correcto: \"http://172.30.2.203:32789\". " +
        "Si corres esto vía Docker, revisa la variable de entorno API_BASE_URL del contenedor " +
        "nexus.presentation (ver DOCKER.md).");
}

// ---------------------------------------------------------------------------
// MudBlazor
// ---------------------------------------------------------------------------
builder.Services.AddMudServices();

// ---------------------------------------------------------------------------
// Autenticación basada en JWT (custom, sin Identity)
// ---------------------------------------------------------------------------
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped(sp => (CustomAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddTransient<JwtAuthorizationMessageHandler>();

// ---------------------------------------------------------------------------
// HttpClient hacia la API de Nexus, con el token JWT inyectado automáticamente
// ---------------------------------------------------------------------------
builder.Services.AddHttpClient("NexusApi", client =>
    {
        client.BaseAddress = parsedApiBaseUrl;
    })
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NexusApi"));

// ---------------------------------------------------------------------------
// Servicios de autenticación y de acceso a la API por módulo
// ---------------------------------------------------------------------------
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmpresaApiService>();
builder.Services.AddScoped<EmpresaConectorApiService>();
builder.Services.AddScoped<UnoEConsultaApiService>();
builder.Services.AddScoped<ConectorApiService>();
builder.Services.AddScoped<IntegracionApiService>();
builder.Services.AddScoped<IntegracionConectorApiService>();
builder.Services.AddScoped<ConfiguracionEnrutamientoApiService>();
builder.Services.AddScoped<AplicacionApiService>();
builder.Services.AddScoped<AplicacionIntegracionApiService>();
builder.Services.AddScoped<AplicacionEmpresaApiService>();
builder.Services.AddScoped<AplicacionConectorApiService>();

await builder.Build().RunAsync();
