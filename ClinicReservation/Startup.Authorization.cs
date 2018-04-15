using AuthenticationCore;
using AuthorizationCore;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Authorizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinicReservation.Services.Data;

namespace ClinicReservation
{
    public partial class Startup
    {
        private void AddAuthorizations(IServiceCollection services)
        {
            services.AddMvcAuthorization<User>(builder =>
            {
                builder.UserAccessor = provider =>
                {
                    IScopedUserAccessor accessor = provider.GetRequiredService<IScopedUserAccessor>();
                    return accessor.User;
                };

                builder.AddHandler<IsActionAllowedPolicy, IsActionAllowedPolicyHandler>();
                builder.AddHandler<Reservation, IsReservationOwnerPolicy, IsReservationOwnerHandler>();
                builder.AddPolicy(new IsActionAllowedPolicy(GroupAction.CreateModifyReservation), "CanCreateReservation");
                builder.AddPolicy(new IsActionAllowedPolicy(GroupAction.ViewAllReservations), "CanViewAllReservations");
                builder.AddPolicy(new IsActionAllowedPolicy(GroupAction.ModifyGroups), "CanModifyGroups");

                builder.AddPolicy<Reservation>(new IsReservationOwnerPolicy(), provider => provider.GetRequiredService<IReservationStore>().Reservation, "IsReservationOwner");

            });
        }
    }
}
