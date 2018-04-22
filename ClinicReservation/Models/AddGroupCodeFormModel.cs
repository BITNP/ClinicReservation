using ClinicReservation.Validates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models
{
    public class QueryGroupCodeFormModel
    {
        [RegularExpression(@"^group_\w+$")]
        [GroupNotExists]
        public string Code { get; set; }
    }

    public class AddGroupCodeFormModel : QueryGroupCodeFormModel
    {
        [StringLength(maximumLength: 24, MinimumLength = 4)]
        [StringNotContains(';')]
        public string Promption { get; set; }
    }
}
