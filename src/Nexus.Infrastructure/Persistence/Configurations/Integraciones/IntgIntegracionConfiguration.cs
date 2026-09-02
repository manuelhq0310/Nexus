using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgIntegracionConfiguration : IEntityTypeConfiguration<IntgIntegracion>
{
    public void Configure(EntityTypeBuilder<IntgIntegracion> builder)
    {
        builder.ToTable("IntgIntegraciones");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).UseIdentityByDefaultColumn();

        builder.Property(i => i.CodigoAccion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Descripcion)
            .HasMaxLength(255);

        builder.Property(i => i.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(i => i.CreatedAt).IsRequired();

        builder.HasIndex(i => i.CodigoAccion).IsUnique();
    }
}
