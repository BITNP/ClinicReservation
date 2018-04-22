using ClinicReservation.Services.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using ClinicReservation.Models.Data;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GroupAttribute : ValidationSetResultAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string group))
                return new ValidationResult("The input value is not a validate string");

            IDbQuery query = validationContext.GetService<IDbQuery>();
            UserGroup groupInstance = query.TryGetGroupByCode(group);
            if (groupInstance == null)
                return new ValidationResult($"The requested group with code {group} is not found in database");

            TrySetResult(validationContext, groupInstance);
            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GroupNotExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string group))
                return new ValidationResult("The input value is not a validate string");

            IDbQuery query = validationContext.GetService<IDbQuery>();
            UserGroup groupInstance = query.TryGetGroupByCode(group);
            if (groupInstance != null)
                return new ValidationResult($"The requested group with code {group} has been existed in database");
            return ValidationResult.Success;
        }
    }
}
