using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPhishing.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ai_phishing");

            migrationBuilder.CreateTable(
                name: "attacks",
                schema: "ai_phishing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    language = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    template = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attacks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "attack_emails",
                schema: "ai_phishing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    attack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    from = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    display_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    to = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_opened = table.Column<bool>(type: "boolean", nullable: false),
                    opened_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_clicked = table.Column<bool>(type: "boolean", nullable: false),
                    clicked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    try_count = table.Column<int>(type: "integer", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attack_emails", x => x.id);
                    table.ForeignKey(
                        name: "fk_attack_emails_attacks_attack_id",
                        column: x => x.attack_id,
                        principalSchema: "ai_phishing",
                        principalTable: "attacks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attack_targets",
                schema: "ai_phishing",
                columns: table => new
                {
                    attack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    attack_type = table.Column<string>(type: "text", nullable: true),
                    target_full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    succeeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attack_targets", x => new { x.attack_id, x.target_email });
                    table.ForeignKey(
                        name: "fk_attack_targets_attacks_attack_id",
                        column: x => x.attack_id,
                        principalSchema: "ai_phishing",
                        principalTable: "attacks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attack_email_replies",
                schema: "ai_phishing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    attack_email_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attack_email_replies", x => x.id);
                    table.ForeignKey(
                        name: "fk_attack_email_replies_attack_emails_attack_email_id",
                        column: x => x.attack_email_id,
                        principalSchema: "ai_phishing",
                        principalTable: "attack_emails",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_attack_email_replies_attack_email_id",
                schema: "ai_phishing",
                table: "attack_email_replies",
                column: "attack_email_id");

            migrationBuilder.CreateIndex(
                name: "ix_attack_emails_attack_id",
                schema: "ai_phishing",
                table: "attack_emails",
                column: "attack_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attack_email_replies",
                schema: "ai_phishing");

            migrationBuilder.DropTable(
                name: "attack_targets",
                schema: "ai_phishing");

            migrationBuilder.DropTable(
                name: "attack_emails",
                schema: "ai_phishing");

            migrationBuilder.DropTable(
                name: "attacks",
                schema: "ai_phishing");
        }
    }
}
