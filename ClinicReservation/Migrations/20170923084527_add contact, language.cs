using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ClinicReservation.Migrations
{
    public partial class addcontactlanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedLanguage",
                table: "ReservationDetails",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contact",
                table: "DutyMembers",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedLanguage",
                table: "ReservationDetails");

            migrationBuilder.DropColumn(
                name: "Contact",
                table: "DutyMembers");
        }
    }
}
