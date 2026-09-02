using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Application.Services;
using Nexus.Infrastructure.Persistence;
using Nexus.Infrastructure.Repositories;
using Nexus.Infrastructure.Security;

namespace Nexus.Infrastructure;

/// <summary>
/// Punto único de registro de dependencias de Infrastructure (y de los servicios
/// de Application que Infrastructure implementa), para mantener Program.cs limpio.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Base de datos: EF Core Code First contra PostgreSQL (NexusDB)
        services.AddDbContext<NexusDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("NexusDB"),
                npgsql => npgsql.MigrationsAssembly(typeof(NexusDbContext).Assembly.FullName)));

        // JWT settings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Repositorios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IConectorRepository, ConectorRepository>();
        services.AddScoped<IIntegracionRepository, IntegracionRepository>();
        services.AddScoped<IIntegracionConectorRepository, IntegracionConectorRepository>();
        services.AddScoped<IEmpresaIntegracionConectorRepository, EmpresaIntegracionConectorRepository>();
        services.AddScoped<IAplicacionRepository, AplicacionRepository>();
        services.AddScoped<IAplicacionIntegracionRepository, AplicacionIntegracionRepository>();
        services.AddScoped<IAplicacionEmpresaRepository, AplicacionEmpresaRepository>();
        services.AddScoped<IAplicacionConectorRepository, AplicacionConectorRepository>();
        services.AddScoped<IEmpresaConectorRepository, EmpresaConectorRepository>();

        // Seguridad
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();

        // Servicios de aplicación (implementación vive en Application, se registra aquí
        // para mantener la composición de dependencias centralizada en Infrastructure).
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmpresaService, EmpresaService>();
        services.AddScoped<IConectorService, ConectorService>();
        services.AddScoped<IIntegracionService, IntegracionService>();
        services.AddScoped<IIntegracionConectorService, IntegracionConectorService>();
        services.AddScoped<IConfiguracionEnrutamientoService, ConfiguracionEnrutamientoService>();
        services.AddScoped<IAplicacionService, AplicacionService>();
        services.AddScoped<IAplicacionIntegracionService, AplicacionIntegracionService>();
        services.AddScoped<IAplicacionEmpresaService, AplicacionEmpresaService>();
        services.AddScoped<IAplicacionConectorService, AplicacionConectorService>();
        services.AddScoped<IEmpresaConectorService, EmpresaConectorService>();

        return services;
    }
}
