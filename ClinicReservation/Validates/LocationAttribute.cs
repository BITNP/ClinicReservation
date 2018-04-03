using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Services;
using ClinicReservation.Models.Data;
using System.Reflection;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LocationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string location))
                return new ValidationResult("The input value is not a validate string");

            IDbQuery query = validationContext.GetService<IDbQuery>();
            Location locationInstance = query.TryGetLocation(location);
            if (locationInstance == null)
                return new ValidationResult($"The requested location with key {location} is not found in database");

            string name = validationContext.MemberName + "Instance";
            PropertyInfo property = validationContext.ObjectType.GetProperty(name);

            if (property != null && property.CanWrite)
                property.SetMethod.Invoke(validationContext.ObjectInstance, new object[] { locationInstance });
            return ValidationResult.Success;
        }
    }
}
