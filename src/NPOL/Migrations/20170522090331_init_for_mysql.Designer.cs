using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NPOL.Models.Reservation;

namespace NPOL.Migrations
{
    [DbContext(typeof(ReservationDbContext))]
    [Migration("20170522090331_init_for_mysql")]
    partial class init_for_mysql
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("NPOL.Models.Reservation.DutyMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Grade")
                        .HasMaxLength(8);

                    b.Property<string>("IconName")
                        .HasMaxLength(64);

                    b.Property<string>("LoginName")
                        .HasMaxLength(64);

                    b.Property<string>("LoginPwd")
                        .HasMaxLength(32);

                    b.Property<string>("Name")
                        .HasMaxLength(64);

                    b.Property<int?>("SchoolId");

                    b.Property<int>("Sexual");

                    b.HasKey("Id");

                    b.HasIndex("SchoolId");

                    b.ToTable("DutyMembers");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.LocationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("LocationTypes");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.ProblemType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasMaxLength(64);

                    b.Property<string>("Name")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("ProblemTypes");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.ReservationBoardMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("DutyMemberId");

                    b.Property<bool>("IsPublic");

                    b.Property<string>("Message")
                        .HasMaxLength(256);

                    b.Property<DateTime>("PostedTime");

                    b.Property<int?>("ReservationDetailId");

                    b.HasKey("Id");

                    b.HasIndex("DutyMemberId");

                    b.HasIndex("ReservationDetailId");

                    b.ToTable("ReservationBoardMessages");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.ReservationDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ActionDate");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Detail")
                        .HasMaxLength(512);

                    b.Property<int?>("DutyMemberId");

                    b.Property<int?>("LocationTypeId");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("PosterEmail")
                        .HasMaxLength(64);

                    b.Property<string>("PosterName")
                        .HasMaxLength(64);

                    b.Property<string>("PosterPhone")
                        .HasMaxLength(20);

                    b.Property<string>("PosterQQ")
                        .HasMaxLength(64);

                    b.Property<int?>("PosterSchoolTypeId");

                    b.Property<int?>("ProblemTypeId");

                    b.Property<DateTime>("ReservationDate");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.HasIndex("DutyMemberId");

                    b.HasIndex("LocationTypeId");

                    b.HasIndex("PosterSchoolTypeId");

                    b.HasIndex("ProblemTypeId");

                    b.ToTable("ReservationDetails");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.SchoolType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("SchoolTypes");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.ServiceFeedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .HasMaxLength(256);

                    b.Property<int>("Rate");

                    b.Property<int>("ReservationDetailId");

                    b.HasKey("Id");

                    b.HasIndex("ReservationDetailId")
                        .IsUnique();

                    b.ToTable("ServiceFeedBacks");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.DutyMember", b =>
                {
                    b.HasOne("NPOL.Models.Reservation.SchoolType", "School")
                        .WithMany("DutyMembers")
                        .HasForeignKey("SchoolId");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.ReservationBoardMessage", b =>
                {
                    b.HasOne("NPOL.Models.Reservation.DutyMember", "DutyMember")
                        .WithMany("ReservationBoardMessages")
                        .HasForeignKey("DutyMemberId");

                    b.HasOne("NPOL.Models.Reservation.ReservationDetail", "ReservationDetail")
                        .WithMany("ReservationBoardMessages")
                        .HasForeignKey("ReservationDetailId");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.ReservationDetail", b =>
                {
                    b.HasOne("NPOL.Models.Reservation.DutyMember", "DutyMember")
                        .WithMany("ReservationDetails")
                        .HasForeignKey("DutyMemberId");

                    b.HasOne("NPOL.Models.Reservation.LocationType", "LocationType")
                        .WithMany("ReservationDetails")
                        .HasForeignKey("LocationTypeId");

                    b.HasOne("NPOL.Models.Reservation.SchoolType", "PosterSchoolType")
                        .WithMany("ReservationDetails")
                        .HasForeignKey("PosterSchoolTypeId");

                    b.HasOne("NPOL.Models.Reservation.ProblemType", "ProblemType")
                        .WithMany("ReservationDetails")
                        .HasForeignKey("ProblemTypeId");
                });

            modelBuilder.Entity("NPOL.Models.Reservation.ServiceFeedback", b =>
                {
                    b.HasOne("NPOL.Models.Reservation.ReservationDetail", "ReservationDetail")
                        .WithOne("Feedback")
                        .HasForeignKey("NPOL.Models.Reservation.ServiceFeedback", "ReservationDetailId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
