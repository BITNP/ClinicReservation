using LocalizationCore.CodeMatching;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models.Data
{
    public class UserGroup : ICodedItem
    {
        [Key, Required]
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        public string Code { get; set; }

        [Required]
        [StringLength(64)]
        public string DefaultName { get; set; }


        public ICollection<UserGroupUser> Users { get; set; }

        [NotMapped]
        public string DisplayName { get; set; }
    }
}
