using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgAplicacionConfiguration : IEntityTypeConfiguration<IntgAplicacion>
{
    public void Configure(EntityTypeBuilder<IntgAplicacion> builder)
    {
        builder.ToTable("IntgAplicacion");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).UseIdentityByDefaultColumn();

        builder.Property(a => a.Nombre).IsRequired();
        builder.Property(a => a.CodigoApp).IsRequired();
        builder.Property(a => a.Descripcion).IsRequired();

        builder.Property(a => a.Estado).IsRequired().HasDefaultValue(true);

        builder.HasIndex(a => a.CodigoApp).IsUnique();

        // M2M Credentials (Opcionales)
        builder.Property(a => a.ClientId).HasMaxLength(100).IsRequired(false);
        builder.Property(a => a.ClientSecretHash).HasMaxLength(255).IsRequired(false);
        builder.Property(a => a.UltimaRotacionSecreto).HasColumnType("timestamp with time zone");

        // Onboarding (Opcionales)
        builder.Property(a => a.OnboardingToken).HasMaxLength(100).IsRequired(false);
        builder.Property(a => a.FechaExpiracionOnboarding).HasColumnType("timestamp with time zone");
        builder.Property(a => a.OnboardingCompletado).HasDefaultValue(false);

        // Índices condicionales / opcionales
        builder.HasIndex(a => a.ClientId).IsUnique().HasFilter("\"ClientId\" IS NOT NULL");
        builder.HasIndex(a => a.OnboardingToken).HasFilter("\"OnboardingToken\" IS NOT NULL");
    }
}
