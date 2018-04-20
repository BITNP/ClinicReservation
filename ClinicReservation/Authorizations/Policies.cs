using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Authorizations
{
    public static class Policies
    {
        public const string CanCreateReservation = "CanCreateReservation";
        public const string CanManageAllReservations = "CanManageAllReservations";
        public const string CanModifyGroups = "CanModifyGroups";
        public const string CanChangeServiceState = "CanChangeServiceState";

        public const string IsCurrentReservationOwner = "IsCurrentReservationOwner";
        public const string CanModifyCurrentReservation = "CanModifyCurrentReservation";
        public const string CanViewCurrentReservation = "CanViewCurrentReservation";

        public const string IsCustomReservationOwner = "IsCustomReservationOwner";
        public const string CanModifyCustomReservation = "CanModifyCustomReservation";
        public const string CanViewCustomReservation = "CanViewCustomReservation";
    }
}
