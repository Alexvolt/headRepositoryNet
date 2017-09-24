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
        [HttpPost("/authenticate")]
        public async Task Authenticate([FromBody] UserPass userPass)
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

        [HttpPost("/accessToken")]
        public async Task GetAccessToken([FromBody] UserPass userPass)
        {
            /* 
                // get auth token data
            let decoded = {}
            try {
                decoded = jwt.verify(req.body.tokenAuth, config.secretAuth);
            } catch (err) {
                res.status(401).send(errorService.userErrorForSending('You need to login again'));
                return;
            }

            // get user data 
            userService.getById(decoded.sub)
                .then((user) => {
                    if (user.haveAccess) {
                        // user haveAccess - send new access token
                        let tokenAccess = userService.getAccessToken(user.id, user.admin);
                        res.send({ tokenAccess: tokenAccess });
                    } else
                        res.status(403).send(errorService.userErrorForSending('access denied by admin'));
                })
                .catch(function (err) {
                    res.status(400).send(errorService.errorForSending(err));
                });*/

        }

        

        /* 
        router.post('/accessToken', getAccessToken);
router.post('/register', register);
router.get('/', getAll);
router.get('/current', getCurrent);
router.get('/:_id', getById);
router.put('/updatePassword/currentUser', updatePasswordCurrentUser);
router.put('/updatePassword/:_id', updatePassword);
router.put('/:_id', update);
router.delete('/:_id', _delete);

        */

        // GET api/users
        [HttpGet]
        public async Task<IEnumerable<User>> Get([FromQuery]Dictionary<string, string> queryParams)
        {
            return await usersRepository.FindAsync(queryParams);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
