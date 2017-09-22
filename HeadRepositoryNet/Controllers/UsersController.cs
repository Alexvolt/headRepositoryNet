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

namespace HeadRepositoryNet.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {

        private readonly UsersRepository usersRepository;
        /*public UsersController(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }*/
        public UsersController(IOptions<DataAccessOptions> dataAccessOptions)
        {
            usersRepository = new UsersRepository(dataAccessOptions);
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<User>> Get([FromQuery]Dictionary<string, string> queryParams)
        {
            return await usersRepository.FindAllAsync(queryParams);
            //return await usersRepository.FindAllAsync();
            //return new string[] { "value1", "value2" };
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
