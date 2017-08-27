using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NPOL.Mid
{
    public class NPOLJwtDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string algorithm;
        private readonly TokenValidationParameters validationParam;

        public NPOLJwtDataFormat(string algorithm, TokenValidationParameters param)
        {
            this.algorithm = algorithm;
            this.validationParam = param;
        }
        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException();
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            return Unprotect(protectedText, null);
        }

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            SecurityToken validToken = null;
            try
            {
                principal = handler.ValidateToken(protectedText, this.validationParam, out validToken);
                JwtSecurityToken validJwt = validToken as JwtSecurityToken;
                if (validJwt == null)
                    throw new ArgumentException("Invalid Jwt");

                if (validJwt.Header.Alg.Equals(algorithm, StringComparison.Ordinal) == false)
                    throw new ArgumentException($"Algorithm must be {algorithm} while get {validJwt.Header.Alg}");
            }
            catch(SecurityTokenValidationException)
            {
                return null;
            }
            catch(ArgumentException)
            {
                return null;
            }
            return new AuthenticationTicket(principal, new AuthenticationProperties(), "Cookie");
        }
    }
}
