using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NPOL.Models.Reservation
{
    public class LocationType
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [MaxLength(64)]
        public string Name { get; set; }

        public IList<ReservationDetail> ReservationDetails { get; set; }
    }
}
