using AuthenticationCore;
using ClinicReservation.Models.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services
{
    public interface IDbQuery
    {
        User TryGetUserByName(string username);
        User TryGetUser(IUser user);
        void AddUser(User user);

        void AddReservation(Reservation reservation);

        Department TryGetDepartment(string department);
        Department TryGetDefaultDepartment();
        bool ContainsDepartment(string department);

        UserGroup TryGetNormalUserGroup();
        UserGroup TryGetGroupByCode(string group);

        bool ContainsCategory(string category);
        Category TryGetCategory(string category);

        bool ContainsLocation(string location);
        Location TryGetLocation(string location);

        EntityEntry<T> GetDbEntry<T>(T entity) where T : class;
        void SaveChanges();
    }

    internal sealed class DbQuery : IDbQuery
    {
        private readonly DataDbContext dbContext;

        public DbQuery(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
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
        public UserGroup TryGetGroupByCode(string group)
        {
            return dbContext.UserGroups.FirstOrDefault(x => x.Code == group);
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
    }
}
