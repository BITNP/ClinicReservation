using ClinicReservation.Models.Data;
using ClinicReservation.Validates;
using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Models
{
    public class NewUserFormModel
    {
        [Required]
        public string Name { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string IM { get; set; }

        [Url]
        public string GitHub { get; set; }

        [Required]
        [Department]
        public string Department { get; set; }
        
        public string Code { get; set; }

        public Department DepartmentInstance { get; set; }


        public void Nullify()
        {
            if (string.IsNullOrWhiteSpace(Name))
                Name = null;

            if (string.IsNullOrWhiteSpace(Phone))
                Phone = null;

            if (string.IsNullOrWhiteSpace(Email))
                Email = null;

            if (string.IsNullOrWhiteSpace(IM))
                IM = null;

            if (string.IsNullOrWhiteSpace(GitHub))
                GitHub = null;

            if (string.IsNullOrWhiteSpace(Department))
                Department = null;

            if (string.IsNullOrWhiteSpace(Code))
                Code = null;
        }
        public void Emptify()
        {
            if (Name == null)
                Name = "";
            if (Phone == null)
                Phone = "";
            if (Email == null)
                Email = "";
            if (IM == null)
                IM = "";
            if (GitHub == null)
                GitHub = "";
            if (Department == null)
                Department = "";
            if (Code == null)
                Code = "";
        }
    }
}