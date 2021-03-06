﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models.Reservation
{
    public class SchoolType
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }


        public IList<ReservationDetail> ReservationDetails { get; set; }

        public IList<DutyMember> DutyMembers { get; set; }
    }
}
