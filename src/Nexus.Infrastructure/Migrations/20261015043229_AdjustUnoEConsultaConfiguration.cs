using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustUnoEConsultaConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlantillaParametrosXml",
                schema: "public",
                table: "UnoEConsultaConfig");

            migrationBuilder.RenameColumn(
                name: "ConsultaGenerica",
                table: "IntgIntegraciones",
                newName: "EsAsincrono");

            migrationBuilder.AddColumn<bool>(
                name: "EsConsultaGenerica",
                table: "IntgIntegracionConectores",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UnoEConsultaConfigId",
                table: "IntgIntegracionConectores",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgIntegracionConectores_UnoEConsultaConfigId",
                table: "IntgIntegracionConectores",
                column: "UnoEConsultaConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_IntgIntegracionConectores_UnoEConsultaConfig_UnoEConsultaCo~",
                table: "IntgIntegracionConectores",
                column: "UnoEConsultaConfigId",
                principalSchema: "public",
                principalTable: "UnoEConsultaConfig",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntgIntegracionConectores_UnoEConsultaConfig_UnoEConsultaCo~",
                table: "IntgIntegracionConectores");

            migrationBuilder.DropIndex(
                name: "IX_IntgIntegracionConectores_UnoEConsultaConfigId",
                table: "IntgIntegracionConectores");

            migrationBuilder.DropColumn(
                name: "EsConsultaGenerica",
                table: "IntgIntegracionConectores");

            migrationBuilder.DropColumn(
                name: "UnoEConsultaConfigId",
                table: "IntgIntegracionConectores");

            migrationBuilder.RenameColumn(
                name: "EsAsincrono",
                table: "IntgIntegraciones",
                newName: "ConsultaGenerica");

            migrationBuilder.AddColumn<string>(
                name: "PlantillaParametrosXml",
                schema: "public",
                table: "UnoEConsultaConfig",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
