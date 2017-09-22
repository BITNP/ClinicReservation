using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
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
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{envName}.json", true)
                .Build();
            string connectionString = configurationRoot.GetConnectionString("reservationData");

            DbContextOptionsBuilder<ReservationDbContext> builder = new DbContextOptionsBuilder<ReservationDbContext>();
            builder = builder.UseSqlServer(connectionString);
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
