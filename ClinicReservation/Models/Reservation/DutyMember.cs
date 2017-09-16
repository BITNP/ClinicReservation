using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models.Reservation
{
    public enum Sexual
    {
        Male = 0, Female = 1, Other = 2
    }
    public class DutyMember
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(64)]
        public string LoginName { get; set; }

        [MaxLength(32)]
        public string LoginPwd { get; set; }


        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(64)]
        public string IconName { get; set; }

        [MaxLength(8)]
        public string Grade { get; set; }

        public Sexual Sexual { get; set; }

        public SchoolType School { get; set; }


        public IList<ReservationBoardMessage> ReservationBoardMessages { get; set; }
        public IList<ReservationDetail> ReservationDetails { get; set; }
    }
}
