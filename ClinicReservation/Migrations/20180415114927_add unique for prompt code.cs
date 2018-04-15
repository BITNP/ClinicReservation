using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ClinicReservation.Migrations
{
    public partial class adduniqueforpromptcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_PromptCode",
                table: "UserGroups",
                column: "PromptCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserGroups_PromptCode",
                table: "UserGroups");
        }
    }
}
