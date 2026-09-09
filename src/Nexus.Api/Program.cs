using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nexus.Api.Middleware;
using Nexus.Infrastructure;
using Nexus.Infrastructure.Persistence;
using Nexus.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// 1. Servicios de la aplicación
// ---------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Registro centralizado de Infrastructure (DbContext, repositorios, JWT, hashing, AuthService)
builder.Services.AddInfrastructure(builder.Configuration);

// ---------------------------------------------------------------------------
// 2. Autenticación JWT
// ---------------------------------------------------------------------------
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("La sección 'JwtSettings' no está configurada en appsettings.json.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        // Eventos de diagnóstico: registran en el log la causa EXACTA de un 401,
        // muy útil en desarrollo para depurar problemas de token rápidamente.
        // Se pueden quitar o dejar (no exponen nada al cliente, solo loguean).
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtBearer");
                logger.LogWarning(context.Exception, "Falló la autenticación JWT: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtBearer");
                logger.LogWarning(
                    "Challenge JWT emitido. Error: {Error}, Descripción: {Description}",
                    context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtBearer");
                logger.LogInformation("Token validado correctamente para: {User}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// 3. Swagger / OpenAPI (con soporte de autenticación Bearer)
// ---------------------------------------------------------------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nexus API",
        Version = "v1",
        Description = "API REST del backend de Nexus: autenticación, gestión de usuarios y servicios privados."
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese ÚNICAMENTE el token JWT (sin la palabra 'Bearer', Swagger la agrega automáticamente).",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });

    // Incluye los comentarios XML (summary/response) en la documentación de Swagger.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ---------------------------------------------------------------------------
// 4. CORS (política abierta básica; ajustar según necesidades del cliente)
// ---------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// 5. Pipeline HTTP
// ---------------------------------------------------------------------------

// Middleware global de manejo de excepciones: debe ir lo más arriba posible
// para capturar cualquier error producido en middlewares/handlers posteriores.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nexus API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ---------------------------------------------------------------------------
// 6. Migración automática al iniciar (opcional, pensada para contenedores)
// ---------------------------------------------------------------------------
// Activar solo estableciendo RUN_MIGRATIONS_ON_STARTUP=true (por ejemplo, en
// docker-compose). Por defecto está desactivada: en desarrollo local se sigue
// prefiriendo `dotnet ef database update` de forma explícita.
if (builder.Configuration.GetValue<bool>("RUN_MIGRATIONS_ON_STARTUP"))
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var db = scope.ServiceProvider.GetRequiredService<NexusDbContext>();

    logger.LogInformation("RUN_MIGRATIONS_ON_STARTUP=true: aplicando migraciones pendientes...");
    db.Database.Migrate();
    logger.LogInformation("Migraciones aplicadas correctamente.");
}

app.Run();

// Necesario para poder referenciar Program desde proyectos de pruebas de integración (WebApplicationFactory).
public partial class Program { }
