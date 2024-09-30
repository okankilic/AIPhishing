using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPhishing.Database.Migrations
{
    /// <inheritdoc />
    public partial class AttackClientRelationsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "client_id",
                schema: "ai_phishing",
                table: "attacks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_attacks_client_id",
                schema: "ai_phishing",
                table: "attacks",
                column: "client_id");

            migrationBuilder.AddForeignKey(
                name: "fk_attacks_clients_client_id",
                schema: "ai_phishing",
                table: "attacks",
                column: "client_id",
                principalSchema: "ai_phishing",
                principalTable: "clients",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attacks_clients_client_id",
                schema: "ai_phishing",
                table: "attacks");

            migrationBuilder.DropIndex(
                name: "ix_attacks_client_id",
                schema: "ai_phishing",
                table: "attacks");

            migrationBuilder.DropColumn(
                name: "client_id",
                schema: "ai_phishing",
                table: "attacks");
        }
    }
}
