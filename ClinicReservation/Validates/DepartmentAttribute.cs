using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using ClinicReservation.Models.Data;
using ClinicReservation.Services;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DepartmentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string department))
                return new ValidationResult("The input value is not a validate string");

            IDbQuery query = validationContext.GetService<IDbQuery>();
            if (query.ContainsDepartment(department))
                return ValidationResult.Success;
            return new ValidationResult($"The requested department with key {department} is not found in database");
        }
    }
}
