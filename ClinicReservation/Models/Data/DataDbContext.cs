using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models.Data
{
    public class DataDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataDbContext>
    {
        public DataDbContext CreateDbContext(string[] args)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory + "../../..";
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{"Development"}.json", true)
                .Build();
            string connectionString = configurationRoot.GetConnectionString("reservationData");

            DbContextOptionsBuilder<DataDbContext> builder = new DbContextOptionsBuilder<DataDbContext>();
            builder = builder.UseSqlServer(connectionString);
            return new DataDbContext(builder.Options);
        }

    }

    public class DataDbContext : DbContext
    {
        public DbSet<BoardMessage> Messages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<DutySchedule> Schedules { get; set; }

        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DutySchedule>().HasOne<User>(schedule => schedule.User).WithMany(user => user.Schedules).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasMany<Reservation>(cate => cate.Reservations).WithOne(reservation => reservation.Category).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Category>().HasIndex(category => category.Code).IsUnique();

            modelBuilder.Entity<Location>().HasMany<Reservation>(location => location.Reservations).WithOne(reservation => reservation.Location).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Location>().HasIndex(location => location.Code).IsUnique();

            // feedbacks should be reserved even when a reservation gets deleted
            modelBuilder.Entity<Feedback>().HasOne<Reservation>(feedback => feedback.Reservation).WithOne(reservation => reservation.Feedback).HasForeignKey<Feedback>(feed => feed.ReservationForeignKey).OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Reservation>().HasOne<User>(reservation => reservation.Poster).WithMany(user => user.PostedReservations).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Reservation>().HasOne<User>(reservation => reservation.Duty).WithMany(user => user.DutiedReservations).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<BoardMessage>().HasOne<Reservation>(board => board.Reservation).WithMany(reservation => reservation.Messages).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BoardMessage>().HasOne<User>(board => board.Poster).WithMany(user => user.PostedMessages).OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<User>().HasOne<Department>(user => user.Department).WithMany(department => department.Users).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique();

            modelBuilder.Entity<Department>().HasIndex(department => department.Code).IsUnique();

            modelBuilder.Entity<UserGroup>().HasIndex(usergroup => usergroup.Code).IsUnique();

            modelBuilder.Entity<UserGroupUser>().HasKey(ug => new { ug.GroupId, ug.UserId });
            modelBuilder.Entity<UserGroupUser>().HasOne(ug => ug.Group).WithMany(group => group.Users).HasForeignKey(ug => ug.GroupId);
            modelBuilder.Entity<UserGroupUser>().HasOne(ug => ug.User).WithMany(user => user.Groups).HasForeignKey(ug => ug.UserId);

            base.OnModelCreating(modelBuilder);

        }
    }
}
