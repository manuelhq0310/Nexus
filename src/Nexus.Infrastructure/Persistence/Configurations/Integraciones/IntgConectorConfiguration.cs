using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgConectorConfiguration : IEntityTypeConfiguration<IntgConector>
{
    public void Configure(EntityTypeBuilder<IntgConector> builder)
    {
        builder.ToTable("IntgConectores");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).UseIdentityByDefaultColumn();

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.TipoProtocolo)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.UrlBase)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt).IsRequired();

        builder.HasIndex(c => c.Nombre).IsUnique();
    }
}
