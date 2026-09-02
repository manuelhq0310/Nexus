using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgEmpresaIntegracionConectorConfiguration : IEntityTypeConfiguration<IntgEmpresaIntegracionConector>
{
    public void Configure(EntityTypeBuilder<IntgEmpresaIntegracionConector> builder)
    {
        builder.ToTable("IntgEmpresaIntegracionConectores");

        builder.HasKey(eic => eic.Id);
        builder.Property(eic => eic.Id).UseIdentityByDefaultColumn();

        builder.Property(eic => eic.RequiereAutenticacion)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(eic => eic.Estado)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(eic => eic.CreatedAt).IsRequired();

        // FK -> IntgEmpresas
        builder.HasOne(eic => eic.Empresa)
            .WithMany(e => e.EmpresaIntegracionConectores)
            .HasForeignKey(eic => eic.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK -> IntgIntegracionConectores
        builder.HasOne(eic => eic.IntegracionConector)
            .WithMany(ic => ic.EmpresaIntegracionConectores)
            .HasForeignKey(eic => eic.IntegracionConectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Una empresa no puede tener duplicada la misma combinación integración-conector.
        builder.HasIndex(eic => new { eic.EmpresaId, eic.IntegracionConectorId }).IsUnique();
    }
}
