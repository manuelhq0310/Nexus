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
        client.BaseAddress = new Uri(apiBaseUrl);
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
