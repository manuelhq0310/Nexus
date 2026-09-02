using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgEmpresaConectorConfiguration : IEntityTypeConfiguration<IntgEmpresaConector>
{
    public void Configure(EntityTypeBuilder<IntgEmpresaConector> builder)
    {
        builder.ToTable("IntgEmpresaConector");

        builder.HasKey(ec => ec.Id);
        builder.Property(ec => ec.Id).UseIdentityByDefaultColumn();

        builder.Property(ec => ec.Estado).IsRequired().HasDefaultValue(true);

        builder.HasOne(ec => ec.Empresa)
            .WithMany(e => e.EmpresaConectores)
            .HasForeignKey(ec => ec.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ec => ec.Conector)
            .WithMany(c => c.EmpresaConectores)
            .HasForeignKey(ec => ec.ConectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación 1 a 1: una empresa tiene a lo sumo un registro de conector asociado.
        builder.HasIndex(ec => ec.EmpresaId).IsUnique();
    }
}
