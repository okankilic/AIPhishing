﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPhishing.Database.Migrations
{
    /// <inheritdoc />
    public partial class ClientTargetDepartmentColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "department",
                schema: "ai_phishing",
                table: "client_targets",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "department",
                schema: "ai_phishing",
                table: "client_targets");
        }
    }
}
