using LocalizationCore.CodeMatching;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models.Data
{
    public class User
    {
        [Key, Required]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string Username { get; set; }

        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        public string Avatar { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(64)]
        public string IM { get; set; }

        [StringLength(64)]
        public string GitHub { get; set; }

        [StringLength(64)]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public Department Department { get; set; }

        [Required]
        public DateTime LastSyncTime { get; set; }

        [Required]
        public bool IsPersonalInformationFilled { get; set; }


        [InverseProperty(nameof(Reservation.Poster))]
        public ICollection<Reservation> PostedReservations { get; set; }

        [InverseProperty(nameof(Reservation.Duty))]
        public ICollection<Reservation> DutiedReservations { get; set; }

        public ICollection<UserGroupUser> Groups { get; set; }

        public ICollection<BoardMessage> PostedMessages { get; set; }

        public ICollection<DutySchedule> Schedules { get; set; }
    }
}
