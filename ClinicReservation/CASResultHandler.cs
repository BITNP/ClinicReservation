using AuthenticationCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace ClinicReservation
{
    public class CASResultHandler : ICASResponseHandler
    {
        private static Regex COLON_REPLACER { get; } = new Regex(@"(?<=\</?cas):");

        private static XPathExpression USER_EXP { get; } = XPathExpression.Compile("cas-user");
        private static XPathExpression SUCCESS_EXP { get; } = XPathExpression.Compile("/cas-serviceResponse/cas-authenticationSuccess");
        private static XPathExpression FAILURE_EXP { get; } = XPathExpression.Compile("/cas-serviceResponse/cas-authenticationFailure/@code");

        private readonly ICASOption option;

        public CASResultHandler(ICASOption option)
        {
            this.option = option;
        }

        public IUser Invoke(HttpContext httpContext, string message, string actionUrl, out string redirectUrl)
        {
            message = COLON_REPLACER.Replace(message, "-");
            TextReader textReader = new StringReader(message);
            XPathDocument document = new XPathDocument(textReader);
            XPathNavigator navigator = document.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select(SUCCESS_EXP);
            if (iterator.Count > 0)
            {
                iterator.MoveNext();
                navigator = iterator.Current;
                string user = navigator.SelectSingleNode(USER_EXP).Value;

                httpContext.Session.SetString(option.SessionName, user);
                redirectUrl = actionUrl;
                return null;
            }
            else
            {
                string code = "UNKNOWN_FAILURE";
                iterator = navigator.Select(FAILURE_EXP);
                if (iterator.Count > 0)
                {
                    iterator.MoveNext();
                    code = iterator.Current.Value;
                }
            }
            redirectUrl = null;
            return null;
        }
    }
}
