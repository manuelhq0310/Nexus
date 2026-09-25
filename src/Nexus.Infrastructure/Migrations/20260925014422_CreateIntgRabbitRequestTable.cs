using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateIntgRabbitRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntgRabbitmqRequest",
                schema: "public",
                columns: table => new
                {
                    RabbitmqRequestId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodigoAccionIntegracion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodigoEmpresa = table.Column<int>(type: "integer", nullable: false),
                    ColaRabbitMqDestino = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Pendiente"),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    Intentos = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaProcesado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RespuestaDetalle = table.Column<string>(type: "text", nullable: true),
                    Usuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgRabbitmqRequest", x => x.RabbitmqRequestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntgRabbitmqRequest_CodigoEmpresa",
                schema: "public",
                table: "IntgRabbitmqRequest",
                column: "CodigoEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_IntgRabbitmqRequest_Estado",
                schema: "public",
                table: "IntgRabbitmqRequest",
                column: "Estado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntgRabbitmqRequest",
                schema: "public");
        }
    }
}
