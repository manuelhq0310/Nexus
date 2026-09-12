using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustEmpresaConectorFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntgEmpresaConector_IntgEmpresas_EmpresaId",
                table: "IntgEmpresaConector");

            migrationBuilder.AddForeignKey(
                name: "FK_IntgEmpresas_IntgEmpresaConector_Id",
                table: "IntgEmpresas",
                column: "Id",
                principalTable: "IntgEmpresaConector",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntgEmpresas_IntgEmpresaConector_Id",
                table: "IntgEmpresas");

            migrationBuilder.AddForeignKey(
                name: "FK_IntgEmpresaConector_IntgEmpresas_EmpresaId",
                table: "IntgEmpresaConector",
                column: "EmpresaId",
                principalTable: "IntgEmpresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
