using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPhishing.Database.Migrations
{
    /// <inheritdoc />
    public partial class ConversationTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from ai_phishing.attack_email_replies where 1 = 1;");
            
            migrationBuilder.Sql(@"delete from ai_phishing.attack_emails where 1 = 1;");
            
            migrationBuilder.Sql(@"delete from ai_phishing.attack_targets where 1 = 1;");
            
            migrationBuilder.Sql(@"delete from ai_phishing.attacks where 1 = 1;");
            
            migrationBuilder.DropForeignKey(
                name: "fk_attack_emails_attacks_attack_id",
                schema: "ai_phishing",
                table: "attack_emails");

            migrationBuilder.DropIndex(
                name: "ix_attack_emails_attack_id",
                schema: "ai_phishing",
                table: "attack_emails");

            migrationBuilder.DropTable(
                name: "attack_targets",
                schema: "ai_phishing");

            migrationBuilder.DropColumn(
                name: "attack_id",
                table: "attack_emails",
                schema: "ai_phishing");

            // migrationBuilder.RenameColumn(
            //     name: "attack_id",
            //     schema: "ai_phishing",
            //     table: "attack_emails",
            //     newName: "conversation_id");

            // migrationBuilder.RenameIndex(
            //     name: "ix_attack_emails_attack_id",
            //     schema: "ai_phishing",
            //     table: "attack_emails",
            //     newName: "ix_attack_emails_conversation_id");

            migrationBuilder.AlterColumn<string>(
                name: "department",
                schema: "ai_phishing",
                table: "client_targets",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);
            
            migrationBuilder.AddColumn<Guid>(
                name: "conversation_id",
                schema: "ai_phishing",
                table: "attack_emails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "conversation_id",
                schema: "ai_phishing",
                table: "attack_email_replies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "conversations",
                schema: "ai_phishing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attack_type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    sender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_opened = table.Column<bool>(type: "boolean", nullable: false),
                    is_clicked = table.Column<bool>(type: "boolean", nullable: false),
                    is_replied = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conversations", x => x.id);
                    table.ForeignKey(
                        name: "fk_conversations_attacks_attack_id",
                        column: x => x.attack_id,
                        principalSchema: "ai_phishing",
                        principalTable: "attacks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_conversations_client_targets_client_target_id",
                        column: x => x.client_target_id,
                        principalSchema: "ai_phishing",
                        principalTable: "client_targets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_attack_emails_attack_email_reply_id",
                schema: "ai_phishing",
                table: "attack_emails",
                column: "attack_email_reply_id");
            
            migrationBuilder.CreateIndex(
                name: "ix_attack_emails_conversation_id",
                schema: "ai_phishing",
                table: "attack_emails",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "ix_attack_email_replies_conversation_id",
                schema: "ai_phishing",
                table: "attack_email_replies",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "ix_conversations_attack_id",
                schema: "ai_phishing",
                table: "conversations",
                column: "attack_id");

            migrationBuilder.CreateIndex(
                name: "ix_conversations_client_target_id_attack_id",
                schema: "ai_phishing",
                table: "conversations",
                columns: new[] { "client_target_id", "attack_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_attack_email_replies_conversations_conversation_id",
                schema: "ai_phishing",
                table: "attack_email_replies",
                column: "conversation_id",
                principalSchema: "ai_phishing",
                principalTable: "conversations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_attack_emails_attack_email_replies_attack_email_reply_id",
                schema: "ai_phishing",
                table: "attack_emails",
                column: "attack_email_reply_id",
                principalSchema: "ai_phishing",
                principalTable: "attack_email_replies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_attack_emails_conversations_conversation_id",
                schema: "ai_phishing",
                table: "attack_emails",
                column: "conversation_id",
                principalSchema: "ai_phishing",
                principalTable: "conversations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attack_email_replies_conversations_conversation_id",
                schema: "ai_phishing",
                table: "attack_email_replies");

            migrationBuilder.DropForeignKey(
                name: "fk_attack_emails_attack_email_replies_attack_email_reply_id",
                schema: "ai_phishing",
                table: "attack_emails");

            migrationBuilder.DropForeignKey(
                name: "fk_attack_emails_conversations_conversation_id",
                schema: "ai_phishing",
                table: "attack_emails");

            migrationBuilder.DropTable(
                name: "conversations",
                schema: "ai_phishing");

            migrationBuilder.DropIndex(
                name: "ix_attack_emails_attack_email_reply_id",
                schema: "ai_phishing",
                table: "attack_emails");

            migrationBuilder.DropIndex(
                name: "ix_attack_email_replies_conversation_id",
                schema: "ai_phishing",
                table: "attack_email_replies");

            migrationBuilder.DropColumn(
                name: "conversation_id",
                schema: "ai_phishing",
                table: "attack_email_replies");

            migrationBuilder.RenameColumn(
                name: "conversation_id",
                schema: "ai_phishing",
                table: "attack_emails",
                newName: "attack_id");

            migrationBuilder.RenameIndex(
                name: "ix_attack_emails_conversation_id",
                schema: "ai_phishing",
                table: "attack_emails",
                newName: "ix_attack_emails_attack_id");

            migrationBuilder.AlterColumn<string>(
                name: "department",
                schema: "ai_phishing",
                table: "client_targets",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "attack_targets",
                schema: "ai_phishing",
                columns: table => new
                {
                    attack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    attack_type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    succeeded = table.Column<bool>(type: "boolean", nullable: false),
                    target_full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "fk_attack_emails_attacks_attack_id",
                schema: "ai_phishing",
                table: "attack_emails",
                column: "attack_id",
                principalSchema: "ai_phishing",
                principalTable: "attacks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
