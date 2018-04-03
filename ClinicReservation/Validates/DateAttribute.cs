using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Services;
using System.Reflection;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string date))
                return new ValidationResult("The input value is not a validate string");

            if(DateTime.TryParse(date, out DateTime result))
            {
                string name = validationContext.MemberName + "Instance";
                PropertyInfo property = validationContext.ObjectType.GetProperty(name);

                if (property != null && property.CanWrite)
                    property.SetMethod.Invoke(validationContext.ObjectInstance, new object[] { result });
                return ValidationResult.Success;
            }
            return new ValidationResult($"The string {date} is not a validate date");
        }
    }
}
