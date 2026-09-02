using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones;

public class IntgAplicacionEmpresaConfiguration : IEntityTypeConfiguration<IntgAplicacionEmpresa>
{
    public void Configure(EntityTypeBuilder<IntgAplicacionEmpresa> builder)
    {
        builder.ToTable("IntgAplicacionEmpresa");

        builder.HasKey(ae => ae.Id);
        builder.Property(ae => ae.Id).UseIdentityByDefaultColumn();

        builder.Property(ae => ae.Estado).IsRequired().HasDefaultValue(true);

        builder.HasOne(ae => ae.Aplicacion)
            .WithMany(a => a.AplicacionEmpresas)
            .HasForeignKey(ae => ae.AplicacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ae => ae.Empresa)
            .WithMany(e => e.AplicacionEmpresas)
            .HasForeignKey(ae => ae.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ae => new { ae.AplicacionId, ae.EmpresaId }).IsUnique();
    }
}
