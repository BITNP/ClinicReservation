using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NPOL.Models.Reservation
{
    public class ServiceFeedback
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ReservationDetailId { get; set; }


        [Range(minimum: -2, maximum: 2)]
        public int Rate { get; set; }

        [MaxLength(256)]
        public string Content { get; set; }

        public ReservationDetail ReservationDetail { get; set; }
    }
}
