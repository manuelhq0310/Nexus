using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TablesCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntgAplicacion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    CodigoApp = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgAplicacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntgConectores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoProtocolo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UrlBase = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgConectores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntgEmpresas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoIdentificacion = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NumeroIdentificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NombreRazonSocial = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgEmpresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntgIntegraciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoAccion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgIntegraciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "User"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntgAplicacionConector",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AplicacionId = table.Column<long>(type: "bigint", nullable: false),
                    ConectorId = table.Column<long>(type: "bigint", nullable: false),
                    UsuarioErp = table.Column<string>(type: "text", nullable: true),
                    PasswordErp = table.Column<string>(type: "text", nullable: true),
                    UrlBasePersonalizada = table.Column<string>(type: "text", nullable: true),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    TokenBearer = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgAplicacionConector", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntgAplicacionConector_IntgAplicacion_AplicacionId",
                        column: x => x.AplicacionId,
                        principalTable: "IntgAplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntgAplicacionConector_IntgConectores_ConectorId",
                        column: x => x.ConectorId,
                        principalTable: "IntgConectores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntgAplicacionEmpresa",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AplicacionId = table.Column<long>(type: "bigint", nullable: false),
                    EmpresaId = table.Column<long>(type: "bigint", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgAplicacionEmpresa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntgAplicacionEmpresa_IntgAplicacion_AplicacionId",
                        column: x => x.AplicacionId,
                        principalTable: "IntgAplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntgAplicacionEmpresa_IntgEmpresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "IntgEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntgEmpresaConector",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<long>(type: "bigint", nullable: false),
                    ConectorId = table.Column<long>(type: "bigint", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgEmpresaConector", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntgEmpresaConector_IntgConectores_ConectorId",
                        column: x => x.ConectorId,
                        principalTable: "IntgConectores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntgEmpresaConector_IntgEmpresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "IntgEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntgAplicacionIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AplicacionId = table.Column<long>(type: "bigint", nullable: false),
                    IntegracionId = table.Column<long>(type: "bigint", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgAplicacionIntegracion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntgAplicacionIntegracion_IntgAplicacion_AplicacionId",
                        column: x => x.AplicacionId,
                        principalTable: "IntgAplicacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntgAplicacionIntegracion_IntgIntegraciones_IntegracionId",
                        column: x => x.IntegracionId,
                        principalTable: "IntgIntegraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntgIntegracionConectores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IntegracionId = table.Column<long>(type: "bigint", nullable: false),
                    ConectorId = table.Column<long>(type: "bigint", nullable: false),
                    RutaEndpoint = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ColaRabbitMQDestino = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgIntegracionConectores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntgIntegracionConectores_IntgConectores_ConectorId",
                        column: x => x.ConectorId,
                        principalTable: "IntgConectores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntgIntegracionConectores_IntgIntegraciones_IntegracionId",
                        column: x => x.IntegracionId,
                        principalTable: "IntgIntegraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IntgEmpresaIntegracionConectores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<long>(type: "bigint", nullable: false),
                    IntegracionConectorId = table.Column<long>(type: "bigint", nullable: false),
                    RequiereAutenticacion = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    TokenBearer = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntgEmpresaIntegracionConectores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntgEmpresaIntegracionConectores_IntgEmpresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "IntgEmpresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntgEmpresaIntegracionConectores_IntgIntegracionConectores_~",
                        column: x => x.IntegracionConectorId,
                        principalTable: "IntgIntegracionConectores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntgAplicacion_CodigoApp",
                table: "IntgAplicacion",
                column: "CodigoApp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgAplicacionConector_AplicacionId_ConectorId",
                table: "IntgAplicacionConector",
                columns: new[] { "AplicacionId", "ConectorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgAplicacionConector_ConectorId",
                table: "IntgAplicacionConector",
                column: "ConectorId");

            migrationBuilder.CreateIndex(
                name: "IX_IntgAplicacionEmpresa_AplicacionId_EmpresaId",
                table: "IntgAplicacionEmpresa",
                columns: new[] { "AplicacionId", "EmpresaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgAplicacionEmpresa_EmpresaId",
                table: "IntgAplicacionEmpresa",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_IntgAplicacionIntegracion_AplicacionId_IntegracionId",
                table: "IntgAplicacionIntegracion",
                columns: new[] { "AplicacionId", "IntegracionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgAplicacionIntegracion_IntegracionId",
                table: "IntgAplicacionIntegracion",
                column: "IntegracionId");

            migrationBuilder.CreateIndex(
                name: "IX_IntgConectores_Nombre",
                table: "IntgConectores",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgEmpresaConector_ConectorId",
                table: "IntgEmpresaConector",
                column: "ConectorId");

            migrationBuilder.CreateIndex(
                name: "IX_IntgEmpresaConector_EmpresaId",
                table: "IntgEmpresaConector",
                column: "EmpresaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgEmpresaIntegracionConectores_EmpresaId_IntegracionConec~",
                table: "IntgEmpresaIntegracionConectores",
                columns: new[] { "EmpresaId", "IntegracionConectorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgEmpresaIntegracionConectores_IntegracionConectorId",
                table: "IntgEmpresaIntegracionConectores",
                column: "IntegracionConectorId");

            migrationBuilder.CreateIndex(
                name: "IX_IntgEmpresas_TipoIdentificacion_NumeroIdentificacion",
                table: "IntgEmpresas",
                columns: new[] { "TipoIdentificacion", "NumeroIdentificacion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgIntegracionConectores_ConectorId",
                table: "IntgIntegracionConectores",
                column: "ConectorId");

            migrationBuilder.CreateIndex(
                name: "IX_IntgIntegracionConectores_IntegracionId_ConectorId",
                table: "IntgIntegracionConectores",
                columns: new[] { "IntegracionId", "ConectorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntgIntegraciones_CodigoAccion",
                table: "IntgIntegraciones",
                column: "CodigoAccion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntgAplicacionConector");

            migrationBuilder.DropTable(
                name: "IntgAplicacionEmpresa");

            migrationBuilder.DropTable(
                name: "IntgAplicacionIntegracion");

            migrationBuilder.DropTable(
                name: "IntgEmpresaConector");

            migrationBuilder.DropTable(
                name: "IntgEmpresaIntegracionConectores");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "IntgAplicacion");

            migrationBuilder.DropTable(
                name: "IntgEmpresas");

            migrationBuilder.DropTable(
                name: "IntgIntegracionConectores");

            migrationBuilder.DropTable(
                name: "IntgConectores");

            migrationBuilder.DropTable(
                name: "IntgIntegraciones");
        }
    }
}
