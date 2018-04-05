using Hake.Extension.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ClinicReservation.Services.Cache
{
    public interface IValidatorSetPropertyMethodCache
    {
        MethodInfo Get(Type type, string validatingProperty);
    }
    internal sealed class ValidatorSetPropertyMethodCache : IValidatorSetPropertyMethodCache
    {
        private ICache<string, MethodInfo> cache;

        public ValidatorSetPropertyMethodCache(int capacity)
        {
            cache = new Cache<string, MethodInfo>(capacity);
        }

        public MethodInfo Get(Type type, string validatingProperty)
        {
            string key = $"{type.FullName}.{validatingProperty}";
            return cache.Get(key, k =>
            {
                PropertyInfo property = type.GetProperty($"{validatingProperty}Instance");
                if (property != null && property.CanWrite)
                    return RetrivationResult<MethodInfo>.Create(property.SetMethod);
                return RetrivationResult<MethodInfo>.Create(null);
            });
        }
    }
}
