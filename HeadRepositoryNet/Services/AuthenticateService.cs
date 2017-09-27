using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using HeadRepositoryNet.Controllers;
using HeadRepositoryNet.Entities;
using HeadRepositoryNet.Helpers;
using HeadRepositoryNet.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace HeadRepositoryNet.Services
{
    public class UserPass
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public User User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class AuthenticateService
    {
        private readonly UsersRepository _usersRepository;

        public AuthenticateService(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<AuthResponse> Authenticate(UserPass userPass)
        {
            var username = userPass.Username;
            var password = userPass.Password;

            // find user, check password, get claimes
            User user = await _usersRepository.GetByName(username);
            if (user != null)
            {
                if (!Password.EqualPassword(password, user.Password))
                {
                    return null;
                }
                
                var claimsIdentity = JWTTokens.GetClaimsIdentity(user);

                // Create jwt token for access
                // And one more for access token refrashing
                var now = DateTime.UtcNow;
                var accessToken = JWTTokens.GetToken(claimsIdentity, now, AuthOptions.LifetimeAccess, AuthOptions.KeyAccess);
                var refreshToken = JWTTokens.GetToken(claimsIdentity, now, AuthOptions.LifetimeRefresh, AuthOptions.KeyRefresh);

                // answer
                return new AuthResponse
                {
                    User = user,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

            }
            return null;
        }
    }
}
    