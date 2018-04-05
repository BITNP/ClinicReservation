using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;

namespace ClinicReservation.Validates
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CategoryAttribute : ValidationSetResultAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string category))
                return new ValidationResult("The input value is not a validate string");

            IDbQuery query = validationContext.GetService<IDbQuery>();
            Category categoryInstance = query.TryGetCategory(category);
            if (categoryInstance == null)
                return new ValidationResult($"The requested category with key {category} is not found in database");
            TrySetResult(validationContext, categoryInstance);
            return ValidationResult.Success;
        }
    }
}
