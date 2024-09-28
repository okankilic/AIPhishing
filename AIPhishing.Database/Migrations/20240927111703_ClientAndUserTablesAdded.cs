using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPhishing.Database.Migrations
{
    /// <inheritdoc />
    public partial class ClientAndUserTablesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clients",
                schema: "ai_phishing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "client_targets",
                schema: "ai_phishing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client_targets", x => x.id);
                    table.ForeignKey(
                        name: "fk_client_targets_clients_client_id",
                        column: x => x.client_id,
                        principalSchema: "ai_phishing",
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "ai_phishing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_clients_client_id",
                        column: x => x.client_id,
                        principalSchema: "ai_phishing",
                        principalTable: "clients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_client_targets_client_id_email",
                schema: "ai_phishing",
                table: "client_targets",
                columns: new[] { "client_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_clients_client_name",
                schema: "ai_phishing",
                table: "clients",
                column: "client_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_client_id",
                schema: "ai_phishing",
                table: "users",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "ai_phishing",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "client_targets",
                schema: "ai_phishing");

            migrationBuilder.DropTable(
                name: "users",
                schema: "ai_phishing");

            migrationBuilder.DropTable(
                name: "clients",
                schema: "ai_phishing");
        }
    }
}
