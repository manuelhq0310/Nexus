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

        builder.Property(e => e.CodigoEmpresa)
            .IsRequired();

        builder.Property(e => e.NombreRazonSocial)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt).IsRequired();

        // Una empresa se identifica de forma única por su codigoEmpresa.
        builder.HasIndex(e => e.CodigoEmpresa)
            .IsUnique();
    }
}
