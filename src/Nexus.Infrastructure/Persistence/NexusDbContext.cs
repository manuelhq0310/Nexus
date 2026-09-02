using Microsoft.EntityFrameworkCore;
using Nexus.Domain.Entities;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence;

/// <summary>
/// Contexto de Entity Framework Core para la base de datos NexusDB (Code First).
/// </summary>
public class NexusDbContext : DbContext
{
    public NexusDbContext(DbContextOptions<NexusDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    // Módulo de Integraciones
    public DbSet<IntgEmpresa> IntgEmpresas => Set<IntgEmpresa>();
    public DbSet<IntgConector> IntgConectores => Set<IntgConector>();
    public DbSet<IntgIntegracion> IntgIntegraciones => Set<IntgIntegracion>();
    public DbSet<IntgIntegracionConector> IntgIntegracionConectores => Set<IntgIntegracionConector>();
    public DbSet<IntgEmpresaIntegracionConector> IntgEmpresaIntegracionConectores => Set<IntgEmpresaIntegracionConector>();

    // Módulo de Aplicaciones
    public DbSet<IntgAplicacion> IntgAplicaciones => Set<IntgAplicacion>();
    public DbSet<IntgEmpresaConector> IntgEmpresaConectores => Set<IntgEmpresaConector>();
    public DbSet<IntgAplicacionConector> IntgAplicacionConectores => Set<IntgAplicacionConector>();
    public DbSet<IntgAplicacionIntegracion> IntgAplicacionIntegraciones => Set<IntgAplicacionIntegracion>();
    public DbSet<IntgAplicacionEmpresa> IntgAplicacionEmpresas => Set<IntgAplicacionEmpresa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas las configuraciones (IEntityTypeConfiguration<T>) del ensamblado.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NexusDbContext).Assembly);
    }
}
