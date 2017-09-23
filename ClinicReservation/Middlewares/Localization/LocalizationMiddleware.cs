using ClinicReservation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Middlewares
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context, CultureContext cultureContext)
        {
            CultureExpression cultureExpression = ExtractLanguageFromUrl(context, cultureContext);
            if (cultureExpression == null)
                cultureExpression = ExtractLanguageFromHeader(context, cultureContext);
            if (cultureExpression == null)
                cultureExpression = cultureContext.Options.DefaultLanguage;
            cultureContext.Culture = cultureExpression;
            return next(context);
        }

        private CultureExpression ExtractLanguageFromHeader(HttpContext context, CultureContext cultureContext)
        {
            CultureOptions cultureOptions = cultureContext.Options;
            StringValues languageValues;
            if (!context.Request.Headers.TryGetValue("Accept-Language", out languageValues))
                return null;
            List<string> languageCodes = new List<string>();
            foreach (string lan in languageValues)
                languageCodes.AddRange(lan.Split(';').Select(x => x.ToLower()));
            CultureExpression exp;
            foreach (string lan in languageCodes)
            {
                if (CultureExpression.TryParse(lan, out exp) && cultureOptions.IsLanguageSupported(exp))
                    return exp;
            }
            return null;
        }
        private CultureExpression ExtractLanguageFromUrl(HttpContext context, CultureContext cultureContext)
        {
            CultureOptions cultureOptions = cultureContext.Options;

            string path = context.Request.Path.Value;
            int slashIndex = 1;
            int pathLength = path.Length;
            for (; slashIndex < pathLength; slashIndex++)
                if (path[slashIndex] == '/') break;
            string lang = path.Substring(1, slashIndex - 1).ToLower();
            if (!CultureExpression.TryParse(lang, out CultureExpression cultureExpression))
                return null;

            path = context.Request.Path.Value;
            if (slashIndex < pathLength)
                context.Request.Path = new PathString(path.Substring(slashIndex));
            else
                context.Request.Path = new PathString("/");

            if (!cultureContext.Options.IsLanguageSupported(cultureExpression))
                return null;

            return cultureExpression;
        }
    }
}
