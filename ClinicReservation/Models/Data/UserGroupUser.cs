using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Models.Data
{
    public class UserGroupUser
    {
        public int GroupId { get; set; }
        public UserGroup Group { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
