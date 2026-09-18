using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCodigoEmpresaColumnToEmpresaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntgEmpresas_TipoIdentificacion_NumeroIdentificacion",
                table: "IntgEmpresas");

            migrationBuilder.DropColumn(
                name: "NumeroIdentificacion",
                table: "IntgEmpresas");

            migrationBuilder.DropColumn(
                name: "TipoIdentificacion",
                table: "IntgEmpresas");

            migrationBuilder.AddColumn<int>(
                name: "CodigoEmpresa",
                table: "IntgEmpresas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IntgEmpresas_CodigoEmpresa",
                table: "IntgEmpresas",
                column: "CodigoEmpresa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntgEmpresas_CodigoEmpresa",
                table: "IntgEmpresas");

            migrationBuilder.DropColumn(
                name: "CodigoEmpresa",
                table: "IntgEmpresas");

            migrationBuilder.AddColumn<string>(
                name: "NumeroIdentificacion",
                table: "IntgEmpresas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoIdentificacion",
                table: "IntgEmpresas",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_IntgEmpresas_TipoIdentificacion_NumeroIdentificacion",
                table: "IntgEmpresas",
                columns: new[] { "TipoIdentificacion", "NumeroIdentificacion" },
                unique: true);
        }
    }
}
