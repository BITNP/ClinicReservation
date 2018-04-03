using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Services;
using ClinicReservation.Models.Data;
using System.Reflection;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CategoryAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string category))
                return new ValidationResult("The input value is not a validate string");

            IDbQuery query = validationContext.GetService<IDbQuery>();
            Category categoryInstance = query.TryGetCategory(category);
            if (categoryInstance == null)
                return new ValidationResult($"The requested category with key {category} is not found in database");

            string name = validationContext.MemberName + "Instance";
            PropertyInfo property = validationContext.ObjectType.GetProperty(name);

            if (property != null && property.CanWrite)
                property.SetMethod.Invoke(validationContext.ObjectInstance, new object[] { categoryInstance });
            return ValidationResult.Success;
        }
    }
}
