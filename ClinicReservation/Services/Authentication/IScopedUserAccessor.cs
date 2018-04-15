using AuthenticationCore;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services.Authentication
{
    public interface IScopedUserAccessor
    {
        User User { get; }
    }

    internal sealed class ScopedUserAccessor : IScopedUserAccessor
    {
        private User user;
        public User User => isLoaded ? user : LoadUser();
        private bool isLoaded;
        private readonly IAuthenticationResult authenticationResult;
        private readonly IDbQuery dbQuery;

        public ScopedUserAccessor(IAuthenticationResult authenticationResult, IDbQuery dbQuery)
        {
            this.authenticationResult = authenticationResult;
            this.dbQuery = dbQuery;
        }
        private User LoadUser()
        {
            if (authenticationResult.IsAuthenticated)
            {
                user = dbQuery.TryGetUser(authenticationResult.User);
                isLoaded = true;
                return user;
            }
            else
            {
                isLoaded = true;
                return null;
            }
        }
    }
}
