using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgEmpresaConfiguration : IEntityTypeConfiguration<IntgEmpresa>
{
    public void Configure(EntityTypeBuilder<IntgEmpresa> builder)
    {
        builder.ToTable("IntgEmpresas");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).UseIdentityByDefaultColumn();

        builder.Property(e => e.TipoIdentificacion)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.NumeroIdentificacion)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.NombreRazonSocial)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt).IsRequired();

        // Una empresa se identifica de forma única por tipo + número de identificación.
        builder.HasIndex(e => new { e.TipoIdentificacion, e.NumeroIdentificacion })
            .IsUnique();
    }
}
