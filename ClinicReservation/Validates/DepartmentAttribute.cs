using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DepartmentAttribute : ValidationSetResultAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string department))
                return new ValidationResult("The input value is not a validate string");

            IDbQuery query = validationContext.GetService<IDbQuery>();
            Department departmentInstance = query.TryGetDepartment(department);
            if (departmentInstance == null)
                return new ValidationResult($"The requested department with key {department} is not found in database");

            TrySetResult(validationContext, departmentInstance);
            return ValidationResult.Success;
        }
    }
}
