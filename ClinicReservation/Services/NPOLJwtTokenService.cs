using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClinicReservation.Services
{
    public class NPOLJwtTokenService
    {
        private JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        private readonly TokenProviderOptions _options;

        public NPOLJwtTokenService(IOptions<TokenProviderOptions> options)
        {
            _options = options.Value;
        }

        public string Encrypt(Dictionary<string, string> options)
        {
            StringBuilder stb = new StringBuilder();
            foreach(KeyValuePair<string, string> pair in options)
                stb.AppendFormat("{0}:{1};", pair.Key, pair.Value);
            string text = stb.ToString();
            DateTime now = DateTime.Now;
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, text)
            };
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials
            );
            return handler.WriteToken(token);
        }
        public Dictionary<string, string> Decrypt(string protectedtoken)
        {
            SecurityToken token;
            ClaimsPrincipal principal = null;

            principal = handler.ValidateToken(protectedtoken, _options.ValidationParameters, out token);
            JwtSecurityToken jwttoken = token as JwtSecurityToken;

            Dictionary<string, string> result = new Dictionary<string, string>();
            Claim claim = jwttoken.Claims.FirstOrDefault(cl => cl.Type == JwtRegisteredClaimNames.Sub);
            if (claim == null)
                return result;

            string[] datas = claim.Value.Split(';');
            int index;
            foreach(string strdata in datas)
            {
                index = strdata.IndexOf(':');
                if(index > 0 && index + 1 < strdata.Length)
                    result.Add(strdata.Substring(0, index), strdata.Substring(index + 1));
            }
            return result;

        }
    }
}
