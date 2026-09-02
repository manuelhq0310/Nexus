using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgIntegracionConectorConfiguration : IEntityTypeConfiguration<IntgIntegracionConector>
{
    public void Configure(EntityTypeBuilder<IntgIntegracionConector> builder)
    {
        builder.ToTable("IntgIntegracionConectores");

        builder.HasKey(ic => ic.Id);
        builder.Property(ic => ic.Id).UseIdentityByDefaultColumn();

        builder.Property(ic => ic.RutaEndpoint)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(ic => ic.ColaRabbitMQDestino)
            .HasMaxLength(100);

        builder.Property(ic => ic.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ic => ic.CreatedAt).IsRequired();

        // FK -> IntgIntegraciones
        builder.HasOne(ic => ic.Integracion)
            .WithMany(i => i.IntegracionConectores)
            .HasForeignKey(ic => ic.IntegracionId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK -> IntgConectores
        builder.HasOne(ic => ic.Conector)
            .WithMany(c => c.IntegracionConectores)
            .HasForeignKey(ic => ic.ConectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Una integración no puede resolverse dos veces por el mismo conector.
        builder.HasIndex(ic => new { ic.IntegracionId, ic.ConectorId }).IsUnique();
    }
}
