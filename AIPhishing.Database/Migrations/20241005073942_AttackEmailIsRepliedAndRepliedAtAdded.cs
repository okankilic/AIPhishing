using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPhishing.Database.Migrations
{
    /// <inheritdoc />
    public partial class AttackEmailIsRepliedAndRepliedAtAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_replied",
                schema: "ai_phishing",
                table: "attack_emails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "replied_at",
                schema: "ai_phishing",
                table: "attack_emails",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_replied",
                schema: "ai_phishing",
                table: "attack_emails");

            migrationBuilder.DropColumn(
                name: "replied_at",
                schema: "ai_phishing",
                table: "attack_emails");
        }
    }
}
