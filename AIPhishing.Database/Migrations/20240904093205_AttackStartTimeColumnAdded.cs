using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPhishing.Database.Migrations
{
    /// <inheritdoc />
    public partial class AttackStartTimeColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "start_time",
                schema: "ai_phishing",
                table: "attacks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "send_at",
                schema: "ai_phishing",
                table: "attack_emails",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "start_time",
                schema: "ai_phishing",
                table: "attacks");

            migrationBuilder.DropColumn(
                name: "send_at",
                schema: "ai_phishing",
                table: "attack_emails");
        }
    }
}
