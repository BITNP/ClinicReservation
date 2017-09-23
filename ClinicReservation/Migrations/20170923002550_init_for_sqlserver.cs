using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ClinicReservation.Migrations
{
    public partial class init_for_sqlserver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProblemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DutyMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Grade = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    IconName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LoginName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LoginPwd = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SchoolId = table.Column<int>(type: "int", nullable: true),
                    Sexual = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutyMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DutyMembers_SchoolTypes_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "SchoolTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DutyMemberId = table.Column<int>(type: "int", nullable: true),
                    LocationTypeId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PosterEmail = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PosterName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PosterPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PosterQQ = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PosterSchoolTypeId = table.Column<int>(type: "int", nullable: true),
                    ProblemTypeId = table.Column<int>(type: "int", nullable: true),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationDetails_DutyMembers_DutyMemberId",
                        column: x => x.DutyMemberId,
                        principalTable: "DutyMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationDetails_LocationTypes_LocationTypeId",
                        column: x => x.LocationTypeId,
                        principalTable: "LocationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationDetails_SchoolTypes_PosterSchoolTypeId",
                        column: x => x.PosterSchoolTypeId,
                        principalTable: "SchoolTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationDetails_ProblemTypes_ProblemTypeId",
                        column: x => x.ProblemTypeId,
                        principalTable: "ProblemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationBoardMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DutyMemberId = table.Column<int>(type: "int", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PostedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationDetailId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationBoardMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationBoardMessages_DutyMembers_DutyMemberId",
                        column: x => x.DutyMemberId,
                        principalTable: "DutyMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationBoardMessages_ReservationDetails_ReservationDetailId",
                        column: x => x.ReservationDetailId,
                        principalTable: "ReservationDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceFeedBacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    ReservationDetailId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFeedBacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceFeedBacks_ReservationDetails_ReservationDetailId",
                        column: x => x.ReservationDetailId,
                        principalTable: "ReservationDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DutyMembers_SchoolId",
                table: "DutyMembers",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationBoardMessages_DutyMemberId",
                table: "ReservationBoardMessages",
                column: "DutyMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationBoardMessages_ReservationDetailId",
                table: "ReservationBoardMessages",
                column: "ReservationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationDetails_DutyMemberId",
                table: "ReservationDetails",
                column: "DutyMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationDetails_LocationTypeId",
                table: "ReservationDetails",
                column: "LocationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationDetails_PosterSchoolTypeId",
                table: "ReservationDetails",
                column: "PosterSchoolTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationDetails_ProblemTypeId",
                table: "ReservationDetails",
                column: "ProblemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeedBacks_ReservationDetailId",
                table: "ServiceFeedBacks",
                column: "ReservationDetailId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationBoardMessages");

            migrationBuilder.DropTable(
                name: "ServiceFeedBacks");

            migrationBuilder.DropTable(
                name: "ReservationDetails");

            migrationBuilder.DropTable(
                name: "DutyMembers");

            migrationBuilder.DropTable(
                name: "LocationTypes");

            migrationBuilder.DropTable(
                name: "ProblemTypes");

            migrationBuilder.DropTable(
                name: "SchoolTypes");
        }
    }
}
