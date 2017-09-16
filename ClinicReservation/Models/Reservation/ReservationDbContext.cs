using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models.Reservation
{
    // used for design-time
    public class ReservationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ReservationDbContext>
    {
        public ReservationDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ReservationDbContext> builder = new DbContextOptionsBuilder<ReservationDbContext>();
            builder = builder.UseMySql("Server=aws-kr-mysql-5627.ct3t40831ye1.ap-northeast-2.rds.amazonaws.com;Port=3306;Database=bitnp_online_reservation;Uid=bitnp;Pwd=awsKRb1tNP2016;");
            return new ReservationDbContext(builder.Options);
        }
    }
    public class ReservationDbContext : DbContext
    {
        public DbSet<ReservationDetail> ReservationDetails { get; set; }
        public DbSet<LocationType> LocationTypes { get; set; }
        public DbSet<ProblemType> ProblemTypes { get; set; }
        public DbSet<SchoolType> SchoolTypes { get; set; }
        public DbSet<ServiceFeedback> ServiceFeedBacks { get; set; }
        public DbSet<DutyMember> DutyMembers { get; set; }

        public DbSet<ReservationBoardMessage> ReservationBoardMessages { get; set; }

        public ReservationDbContext(DbContextOptions<ReservationDbContext> options) : base(options)
        {

        }
    }
}
