using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services
{
    public class CultureOptions
    {
        private CultureExpression defaultLanguage;
        public CultureExpression DefaultLanguage => CreateDefaultCopy();

        private List<CultureExpression> supportedLanguages;
        public CultureOptions(string defaultLanguage, IEnumerable<string> supportedLanguages)
        {
            this.defaultLanguage = CultureExpression.Parse(defaultLanguage);
            this.supportedLanguages = supportedLanguages.Select(x => CultureExpression.Parse(x)).ToList();
        }

        public bool IsLanguageSupported(CultureExpression cultureExpression)
        {
            if (cultureExpression.IsAllRegion)
                return supportedLanguages.FirstOrDefault(x => x.LanguageName == cultureExpression.LanguageName) != null;

            return supportedLanguages.FirstOrDefault(x => x.Language == cultureExpression.Language) != null;
        }

        private CultureExpression CreateDefaultCopy()
        {
            return new CultureExpression()
            {
                RawString = defaultLanguage.RawString,
                IsAllRegion = defaultLanguage.IsAllRegion,
                LanguageName = defaultLanguage.LanguageName,
                RegionName = defaultLanguage.RegionName
            };
        }
    }

    public class CultureExpression
    {
        public string RawString { get; set; }

        private string language;
        public string Language => language ?? (language = (IsAllRegion ? LanguageName : $"{LanguageName}-{RegionName}"));
        public string LanguageName { get; set; }
        public string RegionName { get; set; }
        public bool IsAllRegion { get; set; }

        public CultureExpression()
        { }

        public static bool TryParse(string language, out CultureExpression expression)
        {
            if (language.Length == 2)
            {
                expression = new CultureExpression()
                {
                    RawString = language,
                    LanguageName = language.ToLower(),
                    RegionName = "",
                    IsAllRegion = true
                };
                return true;
            }
            else if (language.Length == 4 && language.EndsWith("-*"))
            {
                expression = new CultureExpression()
                {
                    RawString = language,
                    LanguageName = language.Substring(0, 2).ToLower(),
                    RegionName = "",
                    IsAllRegion = true
                };
                return true;
            }
            else if (language.Length == 5 && language[2] == '-')
            {
                expression = new CultureExpression()
                {
                    RawString = language,
                    LanguageName = language.Substring(0, 2).ToLower(),
                    RegionName = language.Substring(3, 2).ToUpper(),
                    IsAllRegion = false
                };
                return true;
            }
            expression = null;
            return false;
        }
        public static CultureExpression Parse(string language)
        {
            if (language.Length == 2)
            {
                return new CultureExpression()
                {
                    RawString = language,
                    LanguageName = language.ToLower(),
                    RegionName = "",
                    IsAllRegion = true
                };
            }
            else if (language.Length == 4 && language.EndsWith("-*"))
            {
                return new CultureExpression()
                {
                    RawString = language,
                    LanguageName = language.Substring(0, 2).ToLower(),
                    RegionName = "",
                    IsAllRegion = true
                };
            }
            else if (language.Length == 5 && language[2] == '-')
            {
                return new CultureExpression()
                {
                    RawString = language,
                    LanguageName = language.Substring(0, 2).ToLower(),
                    RegionName = language.Substring(3, 2).ToUpper(),
                    IsAllRegion = false
                };
            }
            throw new FormatException($"cannot parse {language} as a vaild language pattern");
        }
    }

    public class CultureContext
    {
        public CultureOptions Options { get; }

        public CultureExpression Culture { get; set; }
        public string UrlCultureSpecifier { get; set; }
        public string Action { get; set; }

        public CultureContext(CultureOptions options)
        {
            Options = options;
        }
    }
}
