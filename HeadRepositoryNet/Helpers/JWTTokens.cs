using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeadRepositoryNet.Entities;
using HeadRepositoryNet.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HeadRepositoryNet.Helpers
{
    public class JWTTokens
    {
        private const string SUBCLIMENAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public static string GetToken(ClaimsIdentity claimsIdentity, DateTime begin, int lifetimeMinutes, string key)
        {
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.Issuer,
                    audience: AuthOptions.Audience,
                    notBefore: begin,
                    claims: claimsIdentity.Claims,
                    expires: begin.Add(TimeSpan.FromMinutes(lifetimeMinutes)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public static ClaimsIdentity GetClaimsIdentity(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Admin ? "Admin" : "Default")
                };
            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        public static ClaimsPrincipal ValidateToken(string token, string secretKey)    
        {
           TokenValidationParameters validationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = AuthOptions.Issuer,
                    ValidAudiences = new[] { AuthOptions.Audience },
                    IssuerSigningKeys = new[] {AuthOptions.GetSymmetricSecurityKey(secretKey)}
                };
            
            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            try
            {
                return handler.ValidateToken(token, validationParameters, out validatedToken);
            }
           catch (System.Exception)
            {
                return null;// invalid token
            }           
        }

        public static User GetDataFromClaimsPrincipal(ClaimsPrincipal climesP)    
        {
            return new User
                {
                    Id = Convert.ToInt32(GetClimeValueIfExists(climesP, SUBCLIMENAME)),
                    Username = GetClimeValueIfExists(climesP, ClaimsIdentity.DefaultNameClaimType),
                    Admin = GetClimeValueIfExists(climesP, ClaimsIdentity.DefaultRoleClaimType) == "Admin" ? true : false
                };
        }

        private static string GetClimeValueIfExists(ClaimsPrincipal climesP, string key)
        {
            var claim = climesP.FindFirst(key);
            if (claim != null)
            {
                return claim.Value;  
            }
            return "";
        }
        
    }
}