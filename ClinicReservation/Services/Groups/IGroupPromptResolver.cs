using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicReservation.Services.Groups
{
    public interface IGroupPromptResolver
    {
        IReadOnlyList<UserGroup> Resolve(string promptCode);
    }
}
