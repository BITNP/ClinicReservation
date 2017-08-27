using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPOL.Models.Reservation
{
    public enum ReservationState
    {
        NewlyCreated = 0,
        Cancelled = 1,
        Answered = 2,
        Completed = 3,
        ClosedWithoutComplete = 4
    }
}
