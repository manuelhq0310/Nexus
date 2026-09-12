using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones
{
    public class UnoEConsultaConfigConfiguration : IEntityTypeConfiguration<UnoEConsultaConfig>
    {
        public void Configure(EntityTypeBuilder<UnoEConsultaConfig> builder)
        {
            // Mapeo de la tabla y esquema PostgreSQL
            builder.ToTable("UnoEConsultaConfig", "public");

            // Llave primaria autoincremental
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .UseIdentityAlwaysColumn();

            // Propiedades y restricciones
            builder.Property(e => e.CodigoConsulta)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Descripcion)
                .HasMaxLength(250);

            builder.Property(e => e.NombreConexion)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.IdProveedor)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.PlantillaParametrosXml)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(e => e.Estado)
                .HasDefaultValue(true);

            builder.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            // Índice único para evitar duplicidad de códigos de consulta
            builder.HasIndex(e => e.CodigoConsulta)
                .IsUnique();
        }
    }
}
