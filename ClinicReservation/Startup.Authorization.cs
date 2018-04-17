using AuthorizationCore;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Authorizations;
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
                builder.AddHandler<IsImplicitReservationOwnerPolicy, IsImplicitReservationOwnerHandler>();
                builder.AddHandler<CompositePolicy, CompositePolicyHandler>();

                builder.AddPolicy(
                    policy: new IsActionAllowedPolicy(GroupAction.CreateModifyReservation),
                    name: Policies.CanCreateReservation);
                builder.AddPolicy(
                    policy: new IsActionAllowedPolicy(GroupAction.ManageAllReservations),
                    name: Policies.CanManageAllReservations);
                builder.AddPolicy(
                    policy: new IsActionAllowedPolicy(GroupAction.ModifyGroups),
                    name: Policies.CanModifyGroups);

                builder.AddPolicy(
                    policy: new IsReservationOwnerPolicy(),
                    accessor: provider => provider.GetRequiredService<IReservationStore>().Reservation,
                    name: Policies.IsCustomReservationOwner);

                builder.AddPolicy(
                    policy: new IsImplicitReservationOwnerPolicy("reservation", "id"),
                    name: Policies.IsCurrentReservationOwner);

                builder.AddPolicy(
                    policy: new CompositePolicy(Policies.IsCurrentReservationOwner, Policies.CanManageAllReservations),
                    name: Policies.CanModifyCurrentReservation);
                builder.AddPolicy(
                    policy: new CompositePolicy(Policies.IsCurrentReservationOwner, Policies.CanManageAllReservations),
                    name: Policies.CanViewCurrentReservation);
                builder.AddPolicy(
                    policy: new CompositePolicy(Policies.IsCustomReservationOwner, Policies.CanManageAllReservations),
                    name: Policies.CanModifyCustomReservation);
                builder.AddPolicy(
                    policy: new CompositePolicy(Policies.IsCustomReservationOwner, Policies.CanManageAllReservations),
                    name: Policies.CanViewCustomReservation);
            });
        }
    }
}
