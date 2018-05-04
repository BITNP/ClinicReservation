using AuthenticationCore;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicReservation.Services.Database
{
    internal sealed class DbQuery : IDbQuery
    {
        private readonly DataDbContext dbContext;

        public DbQuery(DataDbContext dbContext)
        {
            this.dbContext = dbContext;

            // check server state
            if (dbContext.ServerStateChangedRecords.FirstOrDefault() == null)
            {
                dbContext.ServerStateChangedRecords.Add(new ServerStateChangedRecord()
                {
                    IsServiceEnabled = true,
                    Reason = "Server Initialized",
                    Time = DateTime.Now
                });
                dbContext.SaveChanges();
            }
        }
        public User TryGetUserByName(string username)
        {
            return dbContext.Users.FirstOrDefault(user => user.Username == username);
        }
        public User TryGetUser(IUser user)
        {
            string name = user.Name;
            return dbContext.Users.FirstOrDefault(u => u.Username == name);
        }
        public void AddUser(User user)
        {
            dbContext.Users.Add(user);
        }
        public void AddUserGroup(User user, UserGroup group)
        {
            UserGroupUser link = new UserGroupUser()
            {
                User = user,
                Group = group
            };
            // dbContext.Entry(user).EnsureReferencesLoaded(nameof(user.Groups));
            if (user.Groups == null)
            {
                user.Groups = new List<UserGroupUser>();
            }
            user.Groups.Add(link);
        }

        public Department TryGetDepartment(string department)
        {
            return dbContext.Departments.FirstOrDefault(dep => dep.Code == department);
        }
        public bool ContainsDepartment(string department)
        {
            return dbContext.Departments.Any(dep => dep.Code == department);
        }
        public Department TryGetDefaultDepartment()
        {
            return TryGetDepartment(Constants.DEFAULT_DEPARTMENT_CODE);
        }

        public UserGroup TryGetNormalUserGroup()
        {
            return TryGetGroupByCode("group_normal");
        }
        public UserGroup TryGetGroupByCode(string code)
        {
            return dbContext.UserGroups.FirstOrDefault(x => x.Code == code);
        }
        public bool HasGroup(string code)
        {
            return dbContext.UserGroups.Any(x => x.Code == code);
        }
        public void AddGroup(UserGroup group)
        {
            dbContext.UserGroups.Add(group);
        }

        public EntityEntry<T> GetDbEntry<T>(T entity) where T : class
        {
            return dbContext.Entry<T>(entity);
        }
        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public bool ContainsCategory(string category)
        {
            return dbContext.Categories.Any(cate => cate.Code == category);
        }
        public Category TryGetCategory(string category)
        {
            return dbContext.Categories.FirstOrDefault(x => x.Code == category);
        }

        public bool ContainsLocation(string location)
        {
            return dbContext.Locations.Any(loc => loc.Code == location);
        }
        public Location TryGetLocation(string location)
        {
            return dbContext.Locations.FirstOrDefault(x => x.Code == location);
        }

        public void AddReservation(Reservation reservation)
        {
            dbContext.Reservations.Add(reservation);
        }

        public Reservation TryGetReservation(int id)
        {
            return dbContext.Reservations.FirstOrDefault(x => x.Id == id);
        }

        public void AddServerStateChangedRecord(ServerStateChangedRecord record)
        {
            dbContext.ServerStateChangedRecords.Add(record);
        }
        public ServerStateChangedRecord RetriveLastServerStateChangedRecord()
        {
            return dbContext.ServerStateChangedRecords.OrderByDescending(record => record.Time).Take(1).First();
        }
    }
}
