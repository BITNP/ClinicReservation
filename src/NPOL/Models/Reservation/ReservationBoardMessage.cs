using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NPOL.Models.Reservation
{
    public class ReservationBoardMessage
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ReservationDetail ReservationDetail { get; set; }

        
        public DutyMember DutyMember { get; set; }

        public DateTime PostedTime { get; set; }

        [MaxLength(256)]
        public string Message { get; set; }

        [NotMapped]
        public bool FromUser { get { return DutyMember == null; } }

        public bool IsPublic { get; set; }
    }
}
