using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgAplicacionConectorConfiguration : IEntityTypeConfiguration<IntgAplicacionConector>
{
    public void Configure(EntityTypeBuilder<IntgAplicacionConector> builder)
    {
        builder.ToTable("IntgAplicacionConector");

        builder.HasKey(ac => ac.Id);
        builder.Property(ac => ac.Id).UseIdentityByDefaultColumn();

        builder.Property(ac => ac.Estado).IsRequired().HasDefaultValue(true);

        builder.HasOne(ac => ac.Aplicacion)
            .WithMany(a => a.AplicacionConectores)
            .HasForeignKey(ac => ac.AplicacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Conector)
            .WithMany(c => c.AplicacionConectores)
            .HasForeignKey(ac => ac.ConectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Una aplicación no puede habilitar dos veces el mismo conector.
        builder.HasIndex(ac => new { ac.AplicacionId, ac.ConectorId }).IsUnique();
    }
}
