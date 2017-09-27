using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using HeadRepositoryNet.Entities;
using HeadRepositoryNet.Services;
using HeadRepositoryNet.Models;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using HeadRepositoryNet.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace HeadRepositoryNet.Controllers
{

    [Route("api/[controller]")]
    public class UsersController : Controller
    {

        private readonly UsersRepository usersRepository;

        public UsersController(IOptions<DataAccessOptions> dataAccessOptions)
        {
            usersRepository = new UsersRepository(dataAccessOptions);
        }

        //post api/users/authenticate
        [HttpPost("authenticate")]
        public async Task Authenticate([FromBody] UserPass userPass)
        {
            if (userPass.Password == null || userPass.Username == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("invalid input data");
                return;
            }

            try
            {
                var authenticateService = new AuthenticateService(usersRepository);
                AuthResponse responseData = await authenticateService.Authenticate(userPass);
    
                if (responseData == null)
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("Invalid username or password.");
                    return;
                }

                // сериализация ответа
                Response.ContentType = "application/json";
                await Response.WriteAsync(JsonConvert.SerializeObject(responseData, new JsonSerializerSettings { Formatting = Formatting.Indented }));
                
            }
            catch (System.Exception  err)
            {
                    Response.StatusCode = 500;
                    await Response.WriteAsync(err.Message);
                    return;
            }
        }

        public class AccessTokenReq
        {
            public string RefreshToken { get; set; }
        }

        [HttpPost("accessToken")]
        public async Task<IActionResult> GetAccessToken([FromBody] AccessTokenReq accessTokenReq)
        {
            if (accessTokenReq.RefreshToken is null)
            {
                return BadRequest();
            }
            string refreshToken = accessTokenReq.RefreshToken;

            var claimsPrincipal = JWTTokens.ValidateToken(refreshToken, AuthOptions.KeyRefresh);
            if (claimsPrincipal == null)
            {
                return StatusCode(401,"You need to login again");
            }
            else
            {
                var userFromToken = JWTTokens.GetDataFromClaimsPrincipal(claimsPrincipal);
                
                // get user data from db
                var user = await usersRepository.GetById(userFromToken.Id);
                if (user.HaveAccess)
                {
                    var claimsIdentity = JWTTokens.GetClaimsIdentity(user);
                    // Create jwt token for access
                    // And one more for access token refrashing
                    var now = DateTime.UtcNow;
                    var accessToken = JWTTokens.GetToken(claimsIdentity, now, AuthOptions.LifetimeAccess, AuthOptions.KeyAccess);
                    return new ObjectResult(new { AccessToken = accessToken });
                }
                else
                    return StatusCode(403,"Access denied by admin");
            }
       }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                await usersRepository.Create(user);
                return Ok();
            }
            catch (System.Exception err)
            {
                
                return StatusCode(500,err.Message);
            }
        }
        
        // GET api/users
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<User>> GetAll([FromQuery] Dictionary<string, string> queryParams)
        {
            return await usersRepository.FindAsync(queryParams);
        }
        
        // GET api/users/current
        [Authorize]
        [HttpGet("current")]
        public async Task<User> GetCurrent()
        {
            var userFromToken = JWTTokens.GetDataFromClaimsPrincipal(HttpContext.User);
            return await usersRepository.GetById(userFromToken.Id);
        }

        // GET api/users/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<User> GetById(int id)
        {
            return await usersRepository.GetById(id);
        }

        // PUT api/users/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]User user)
        {
            var userFromToken = JWTTokens.GetDataFromClaimsPrincipal(HttpContext.User);
            await usersRepository.Update(user, userFromToken.Admin);
        }

        // DELETE api/users/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await usersRepository.Delete(id);
        }
        

        public class PassChangeParams
        {
            public string OldPassword { get; set; }
            public string Password { get; set; }
        }

        // PUT api/users/updatePassword/currentUser
        [Authorize]
        [HttpPut("updatePassword/currentUser")]
        public async Task<IActionResult> UpdatePasswordCurrentUser([FromBody] PassChangeParams passParams)
        {
            
            if (String.IsNullOrEmpty(passParams.OldPassword) || String.IsNullOrEmpty(passParams.Password) )
            {
                return StatusCode(500);
            }
            var userFromToken = JWTTokens.GetDataFromClaimsPrincipal(HttpContext.User);
            var user = await usersRepository.GetById(userFromToken.Id);
            if (Password.EqualPassword(passParams.OldPassword, user.Password))
            {
                await usersRepository.UpdatePassword(userFromToken.Id, passParams.Password);
                return Ok();                
            }
            return StatusCode(403, "Old password is incorrect");            
        }

        // PUT api/users/updatePassword/5
        [Authorize(Roles = "admin")]
        [HttpPut("updatePassword/{id}")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] PassChangeParams passParams)
        {            
            if (String.IsNullOrEmpty(passParams.Password) )
            {
                return StatusCode(500);
            }
            await usersRepository.UpdatePassword(id, passParams.Password);
            return Ok();                
        }
    }
}
