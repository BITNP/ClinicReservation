using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LocationAttribute : ValidationSetResultAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string location))
                return new ValidationResult("The input value is not a validate string");

            IDbQuery query = validationContext.GetService<IDbQuery>();
            Location locationInstance = query.TryGetLocation(location);
            if (locationInstance == null)
                return new ValidationResult($"The requested location with key {location} is not found in database");

            TrySetResult(validationContext, locationInstance);
            return ValidationResult.Success;
        }
    }
}
