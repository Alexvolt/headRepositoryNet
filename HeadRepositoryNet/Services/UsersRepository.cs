using Dapper;
using HeadRepositoryNet.Entities;
using HeadRepositoryNet.Models;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper.FastCrud;
using HeadRepositoryNet.Helpers;

namespace HeadRepositoryNet.Services
{
    public class UsersRepository
    {
        private readonly DataAccessOptions _dataAccessOptions;
        public UsersRepository(IOptions<DataAccessOptions> dataAccessOptions)
        {
            _dataAccessOptions = dataAccessOptions.Value;
        }

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(_dataAccessOptions.PostgresConnectionString);
            }
        }

        public async Task<IEnumerable<User>> FindAllAsync(Dictionary<string, string> queryParams)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //string query = "select \"Id\", \"Username\", \"FirstName\", \"LastName\", \"Email\", \"Admin\", \"HaveAccess\" from \"Users\"" + QueryHelper<User>.QueryOptions(queryParams);
                string query1 = QueryHelper<User>.BuildSelectQuery(queryParams);
                return await dbConnection.QueryAsync<User>(query1);
                
                /*return await dbConnection.FindAsync<User>();*/

                //.QueryAsync<User>("SELECT * FROM users");
            }
        }

/*
        public async Task<IActionResult> GetById(long id)
        {
            var item = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        public IActionResult Create([FromBody] TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.TodoItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = item.IsComplete;
            todo.Name = item.Name;

            _context.TodoItems.Update(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }*/


    }
}
