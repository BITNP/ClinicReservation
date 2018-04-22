using AuthenticationCore;
using ClinicReservation.Models.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicReservation.Services.Database
{
    public interface IDbQuery
    {
        User TryGetUserByName(string username);
        User TryGetUser(IUser user);
        void AddUser(User user);

        void AddUserGroup(User user, UserGroup group);

        void AddReservation(Reservation reservation);
        Reservation TryGetReservation(int id);

        Department TryGetDepartment(string department);
        Department TryGetDefaultDepartment();
        bool ContainsDepartment(string department);

        UserGroup TryGetNormalUserGroup();
        UserGroup TryGetGroupByCode(string code);
        bool HasGroup(string code);
        void AddGroup(UserGroup group);

        bool ContainsCategory(string category);
        Category TryGetCategory(string category);

        bool ContainsLocation(string location);
        Location TryGetLocation(string location);

        EntityEntry<T> GetDbEntry<T>(T entity) where T : class;
        void SaveChanges();
    }
}
