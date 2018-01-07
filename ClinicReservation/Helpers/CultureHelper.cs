using ClinicReservation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Helpers
{
    public static class CultureHelper
    {
        public static T MatchCulture<T>(Dictionary<string, T> dictionary, CultureExpression culture, out CultureExpression matchedCulture)
        {
            string requestedLanguage = culture.Language;
            string lang = requestedLanguage;
            T result;
            if (!culture.IsAllRegion && dictionary.TryGetValue(lang, out result))
            {
                // exact match
                matchedCulture = CultureExpression.Parse(lang);
                return result;
            }
            lang = culture.LanguageName;
            if (dictionary.TryGetValue(lang, out result))
            {
                // language name match
                matchedCulture = CultureExpression.Parse(lang);
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
