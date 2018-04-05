using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ClinicReservation.Services.Cache;

namespace ClinicReservation.Validates
{
#pragma warning disable RCS1203 // Use AttributeUsageAttribute.
    public abstract class ValidationSetResultAttribute : ValidationAttribute
#pragma warning restore RCS1203 // Use AttributeUsageAttribute.
    {
        private object[] param = new object[1];

        protected void TrySetResult(ValidationContext validationContext, object result)
        {
            string name = validationContext.MemberName;
            Type objectType = validationContext.ObjectType;
            IValidatorSetPropertyMethodCache cache = validationContext.GetRequiredService<IValidatorSetPropertyMethodCache>();
            MethodInfo setMethod = cache.Get(objectType, name);
            if (setMethod != null)
            {
                param[0] = result;
                setMethod.Invoke(validationContext.ObjectInstance, param);
            }
        }
    }
}
