using ClinicReservation.Services;
using LocalizationCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Helpers
{
    public static class CultureHelper
    {
        public static T MatchCulture<T>(Dictionary<string, T> dictionary, ICultureExpression culture, out ICultureExpression matchedCulture)
        {
            string requestedLanguage = culture.Language;
            string lang = requestedLanguage;
            T result;
            if (!culture.IsAllRegion && dictionary.TryGetValue(lang, out result))
            {
                // exact match
                matchedCulture = lang.ParseCultureExpression();
                return result;
            }
            lang = culture.Language;
            if (dictionary.TryGetValue(lang, out result))
            {
                // language name match
                matchedCulture = lang.ParseCultureExpression();
                return result;
            }
            else
            {
                matchedCulture = null;
                return default(T);
            }
        }
    }
}
