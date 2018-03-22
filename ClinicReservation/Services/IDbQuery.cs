using ClinicReservation.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services
{
    public interface IDbQuery
    {
        User TryGetUserByName(string username);
        Department TryGetDepartmentByCode(string department);
        bool ContainsDepartment(string department);
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
        public Department TryGetDepartmentByCode(string department)
        {
            return dbContext.Departments.FirstOrDefault(dep => dep.Code == department);
        }
        public bool ContainsDepartment(string department)
        {
            return dbContext.Departments.Any(dep => dep.Code == department);
        }
    }
}
