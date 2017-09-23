using ClinicReservation.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace ClinicReservation.Controllers
{
    public class LanguageRequestResult
    {
        public LanguageRequestResult(string requestedLanguage, string renderedLanguage, bool isRequestSatisfied)
        {
            RequestedLanguage = requestedLanguage;
            RenderedLanguage = renderedLanguage;
            IsRequestSatisfied = isRequestSatisfied;
        }

        public string RequestedLanguage { get; }
        public string RenderedLanguage { get; }
        public bool IsRequestSatisfied { get; }
    }

    public class LocalizedViewFindExecutor : ViewResultExecutor
    {
        public override ViewEngineResult FindView(ActionContext actionContext, ViewResult viewResult)
        {
            string viewName = viewResult.ViewName;
            if (viewName == null)
                viewName = GetActionName(actionContext);
            if (viewName == null)
                return base.FindView(actionContext, viewResult);

            string controllerName;
            if (!actionContext.ActionDescriptor.RouteValues.TryGetValue(ControllerNameKey, out controllerName) ||
                string.IsNullOrEmpty(controllerName))
                controllerName = "";
            string basePath = Path.Combine("Views", controllerName, viewName);

            IServiceProvider services = actionContext.HttpContext.RequestServices;
            CultureContext cultureContext = services.GetRequiredService<CultureContext>();
            CultureExpression cultureExpression = cultureContext.Culture;
            string requestedLanguage = cultureExpression.Language;
            string lang = requestedLanguage;
            string nameResult;
            if (!cultureExpression.IsAllRegion && IsViewFileExisits(lang, out nameResult))
            {
                // exact match
                viewResult.ViewName = nameResult;
                viewResult.ViewData["Language"] = new LanguageRequestResult(lang, lang, true);
                return base.FindView(actionContext, viewResult);
            }
            lang = cultureExpression.LanguageName;
            if (IsViewFileExisits(lang, out nameResult))
            {
                // language name match
                viewResult.ViewName = nameResult;
                viewResult.ViewData["Language"] = new LanguageRequestResult(requestedLanguage, lang, true);
                return base.FindView(actionContext, viewResult);
            }
            else
            {
                // find the first availble region of one language
                IDirectoryContents directoryContents = env.ContentRootFileProvider.GetDirectoryContents(Path.Combine("Views", controllerName));
                string startsWithFilter = $"{viewName}.{lang}";
                IFileInfo file = directoryContents.FirstOrDefault(x => x.Name.StartsWith(startsWithFilter) && x.Name.EndsWith(".cshtml"));
                if (file != null)
                {
                    string cultureName = file.Name.Substring(viewName.Length + 1);
                    cultureName.Substring(0, cultureName.Length - 7);
                    nameResult = file.Name.Substring(0, file.Name.Length - 7);
                    if (CultureExpression.TryParse(cultureName, out CultureExpression exp))
                        viewResult.ViewData["Language"] = new LanguageRequestResult(requestedLanguage, exp.Language, true);
                    else
                        viewResult.ViewData["Language"] = new LanguageRequestResult(requestedLanguage, null, false);
                    viewResult.ViewName = nameResult;
                    return base.FindView(actionContext, viewResult);
                }
            }

            CultureExpression defaultLanguage = cultureContext.Options.DefaultLanguage;
            lang = defaultLanguage.Language;
            if (IsViewFileExisits(lang, out nameResult))
            {
                // default language match
                viewResult.ViewName = nameResult;
                viewResult.ViewData["Language"] = new LanguageRequestResult(requestedLanguage, lang, false);
                return base.FindView(actionContext, viewResult);
            }
            lang = defaultLanguage.LanguageName;
            if (IsViewFileExisits(lang, out nameResult))
            {
                // default language name match
                viewResult.ViewName = nameResult;
                viewResult.ViewData["Language"] = new LanguageRequestResult(requestedLanguage, lang, false);
                return base.FindView(actionContext, viewResult);
            }

            // default file match
            viewResult.ViewName = viewName;
            viewResult.ViewData["Language"] = new LanguageRequestResult(requestedLanguage, null, false);
            return base.FindView(actionContext, viewResult);

            bool IsViewFileExisits(string language, out string viewNameaResult)
            {
                string pathWithLang = $"{basePath}.{lang}.cshtml";
                if (env.ContentRootFileProvider.GetFileInfo(pathWithLang).Exists)
                {
                    viewNameaResult = viewName + "." + lang;
                    return true;
                }
                viewNameaResult = null;
                return false;
            }
        }

        private const string ActionNameKey = "action";
        private const string ControllerNameKey = "controller";
        private readonly IHostingEnvironment env;

        public LocalizedViewFindExecutor(IHostingEnvironment env, IOptions<MvcViewOptions> viewOptions, IHttpResponseStreamWriterFactory writerFactory, ICompositeViewEngine viewEngine, ITempDataDictionaryFactory tempDataFactory, DiagnosticSource diagnosticSource, ILoggerFactory loggerFactory, IModelMetadataProvider modelMetadataProvider) : base(viewOptions, writerFactory, viewEngine, tempDataFactory, diagnosticSource, loggerFactory, modelMetadataProvider)
        {
            this.env = env;
        }

        // copied from asp.net source code
        private static string GetActionName(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.RouteData.Values.TryGetValue(ActionNameKey, out var routeValue))
                return null;

            var actionDescriptor = context.ActionDescriptor;
            string normalizedValue = null;
            if (actionDescriptor.RouteValues.TryGetValue(ActionNameKey, out var value) &&
                !string.IsNullOrEmpty(value))
                normalizedValue = value;

            var stringRouteValue = routeValue?.ToString();
            if (string.Equals(normalizedValue, stringRouteValue, StringComparison.OrdinalIgnoreCase))
                return normalizedValue;

            return stringRouteValue;
        }
    }
    public class LocalizedViewFindResult : ViewResult
    {
        public string Language { get; set; }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            IServiceProvider services = context.HttpContext.RequestServices;
            LocalizedViewFindExecutor executor = services.GetRequiredService<LocalizedViewFindExecutor>();
            ViewEngineResult result = executor.FindView(context, this);
            result.EnsureSuccessful(originalLocations: null);
            IView view = result.View;
            using (view as IDisposable)
            {
                await executor.ExecuteAsync(context, view, this);
            }
        }
    }

    public abstract class LocalizedViewFindableController : Controller
    {
        private readonly string language;
        private readonly CultureContext cultureContext;

        public LocalizedViewFindableController(CultureContext cultureContext)
        {
            language = cultureContext.Culture.Language;
            this.cultureContext = cultureContext;
        }

        public override ViewResult View(string viewName, object model)
        {
            ViewData.Model = model;
            return new LocalizedViewFindResult()
            {
                ViewData = ViewData,
                TempData = TempData,
                ViewName = viewName,
                Language = language
            };
        }

        private string GetCultureDescribledUrl(string url)
        {
            return cultureContext.UrlCultureSpecifier + url;
        }
    }
}
