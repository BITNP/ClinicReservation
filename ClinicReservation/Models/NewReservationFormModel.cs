using ClinicReservation.Models.Data;
using ClinicReservation.Validates;
using DNTCaptcha.Core.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Models
{
    public class NewReservationFormModel : DataWithCaptchaModelBase
    {
        [Required]
        [MinLength(5)]
        public string Detail { get; set; }

        [Required]
        [Location]
        public string Location { get; set; }

        [Required]
        [Category]
        public string Category { get; set; }

        [Required]
        [Date]
        public string BookDate { get; set; }

        public Location LocationInstance { get; set; }
        public Category CategoryInstance { get; set; }
        public DateTime BookDateInstance { get; set; }
    }
}