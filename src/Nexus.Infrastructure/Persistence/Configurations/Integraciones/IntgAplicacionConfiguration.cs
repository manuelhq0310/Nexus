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
    }
}
