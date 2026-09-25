using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Domain.Enums;

namespace Nexus.Infrastructure.Persistence.Configurations.Integraciones
{
    public class IntgRabbitmqRequestConfiguration : IEntityTypeConfiguration<IntgRabbitmqRequest>
    {
        public void Configure(EntityTypeBuilder<IntgRabbitmqRequest> builder)
        {
            builder.ToTable("IntgRabbitmqRequest", "public");

            builder.HasKey(e => e.RabbitmqRequestId);

            builder.Property(e => e.RabbitmqRequestId)
                .HasMaxLength(100);

            builder.Property(e => e.CodigoAccionIntegracion)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.CodigoEmpresa)
                .IsRequired();

            builder.Property(e => e.ColaRabbitMqDestino)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Estado)
                .HasConversion<string>()
                .HasMaxLength(30)
                .HasDefaultValue(EstadoRabbitMqRequest.Pendiente);

            builder.Property(e => e.Payload)
                .HasColumnType("text");

            builder.Property(e => e.Intentos)
                .HasDefaultValue(0);

            builder.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.FechaProcesado)
                .HasColumnType("timestamp with time zone");

            builder.Property(e => e.RespuestaDetalle)
                .HasColumnType("text");

            builder.Property(e => e.Usuario)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(e => e.Estado);
            builder.HasIndex(e => e.CodigoEmpresa);
        }
    }
}
