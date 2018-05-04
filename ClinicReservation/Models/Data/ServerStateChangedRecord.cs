using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models.Data
{
    public class ServerStateChangedRecord
    {
        [Required, Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Reason { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public bool IsServiceEnabled { get; set; }
    }
}
