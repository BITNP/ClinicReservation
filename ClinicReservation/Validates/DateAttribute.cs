using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateAttribute : ValidationSetResultAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string date))
                return new ValidationResult("The input value is not a validate string");

            if (DateTime.TryParse(date, out DateTime result))
            {
                TrySetResult(validationContext, result);
                return ValidationResult.Success;
            }
            return new ValidationResult($"The string {date} is not a validate date");
        }
    }
}
