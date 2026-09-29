using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateClientIdAndClientSecretColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "IntgAplicacion",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecretHash",
                table: "IntgAplicacion",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaExpiracionOnboarding",
                table: "IntgAplicacion",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnboardingCompletado",
                table: "IntgAplicacion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OnboardingToken",
                table: "IntgAplicacion",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaRotacionSecreto",
                table: "IntgAplicacion",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgAplicacion_ClientId",
                table: "IntgAplicacion",
                column: "ClientId",
                unique: true,
                filter: "\"ClientId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IntgAplicacion_OnboardingToken",
                table: "IntgAplicacion",
                column: "OnboardingToken",
                filter: "\"OnboardingToken\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntgAplicacion_ClientId",
                table: "IntgAplicacion");

            migrationBuilder.DropIndex(
                name: "IX_IntgAplicacion_OnboardingToken",
                table: "IntgAplicacion");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "IntgAplicacion");

            migrationBuilder.DropColumn(
                name: "ClientSecretHash",
                table: "IntgAplicacion");

            migrationBuilder.DropColumn(
                name: "FechaExpiracionOnboarding",
                table: "IntgAplicacion");

            migrationBuilder.DropColumn(
                name: "OnboardingCompletado",
                table: "IntgAplicacion");

            migrationBuilder.DropColumn(
                name: "OnboardingToken",
                table: "IntgAplicacion");

            migrationBuilder.DropColumn(
                name: "UltimaRotacionSecreto",
                table: "IntgAplicacion");
        }
    }
}
