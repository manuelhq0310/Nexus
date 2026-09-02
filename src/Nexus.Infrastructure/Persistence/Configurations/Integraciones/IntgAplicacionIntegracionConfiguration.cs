using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgAplicacionIntegracionConfiguration : IEntityTypeConfiguration<IntgAplicacionIntegracion>
{
    public void Configure(EntityTypeBuilder<IntgAplicacionIntegracion> builder)
    {
        builder.ToTable("IntgAplicacionIntegracion");

        builder.HasKey(ai => ai.Id);
        builder.Property(ai => ai.Id).UseIdentityByDefaultColumn();

        builder.Property(ai => ai.Estado).IsRequired().HasDefaultValue(true);

        builder.HasOne(ai => ai.Aplicacion)
            .WithMany(a => a.AplicacionIntegraciones)
            .HasForeignKey(ai => ai.AplicacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ai => ai.Integracion)
            .WithMany(i => i.AplicacionIntegraciones)
            .HasForeignKey(ai => ai.IntegracionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ai => new { ai.AplicacionId, ai.IntegracionId }).IsUnique();
    }
}
