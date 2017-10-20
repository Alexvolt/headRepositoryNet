using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using HeadRepositoryNet.Entities;
using HeadRepositoryNet.Services;
using HeadRepositoryNet.Models;
using Microsoft.AspNetCore.Authorization;

namespace HeadRepositoryNet.Controllers
{
    [Route("api/[controller]")]
    public class ProfessionalAreasController : Controller
    {

        private readonly ProfessionalAreasRepository dataRepository;

        public ProfessionalAreasController(IOptions<DataAccessOptions> dataAccessOptions)
        {
            dataRepository = new ProfessionalAreasRepository(dataAccessOptions);
        }

        // GET: /api/<controller>/parents
        [Authorize]
        [HttpGet("parents")]
        public async Task<IEnumerable<NameWithId>> GetParents()
        {
            return await dataRepository.GetParents();
        }

        // GET: /api/<controller>/
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<ProfessionalArea>> FindAsync([FromQuery] Dictionary<string, string> queryParams)
        {
            return await dataRepository.FindAsync(queryParams);
        }

        // GET api/<controller>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ProfessionalArea> GetById(int id)
        {
            return await dataRepository.GetById(id);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProfessionalArea data)
        {
            try
            {
                await dataRepository.Create(data);
                return Ok();
            }
            catch (System.Exception err)
            {

                return StatusCode(500, err.Message);
            }
        }

        // PUT api/<controller>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]ProfessionalArea data)
        {
            await dataRepository.Update(data);
            return Ok();
        }

        // DELETE api/<controller>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await dataRepository.Delete(id);
        }

    }
}
