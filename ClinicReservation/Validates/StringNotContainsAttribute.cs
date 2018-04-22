using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringNotContainsAttribute : ValidationAttribute
    {
        private readonly char[] chs;

        public StringNotContainsAttribute(params char[] chs)
        {
            if (chs.Length <= 0)
                throw new Exception("char set must contain at least one element");
            this.chs = chs;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string str))
                return new ValidationResult("The input value is not a validate string");

            foreach (char ch in chs)
            {
                if (str.IndexOf(ch) >= 0)
                    return new ValidationResult("One or more unexpected characters are detected");
            }
            return ValidationResult.Success;
        }
    }
}
