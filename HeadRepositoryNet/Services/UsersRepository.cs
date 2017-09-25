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
using System.Linq;
using System;

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

        public async Task<IEnumerable<User>> FindAsync(Dictionary<string, string> queryParams)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                if (queryParams.Count > 0)
                {
                    string query = QueryHelper<User>.BuildSelectQuery(queryParams);
                    return await dbConnection.QueryAsync<User>(query);
                } 
                else
                {
                    return await dbConnection.FindAsync<User>();
                }
            }
        }

        public async Task<User> GetByName(string userName)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var users = await dbConnection.QueryAsync<User>("select * from User where Username = @Username", new { Username = userName });
                return users.FirstOrDefault();
             }
        }

        public async Task<User> GetById(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                return await dbConnection.GetAsync<User>(new User {Id = id});
            }
        }

        public async Task Create(User userParam)
        {
            User user = await GetByName(userParam.Username);
            if (user != null)
            {
                throw new ApplicationException("User name already exists");
            }
            else
            {
                Password pass = new Password(userParam.Password);
                userParam.Password = pass.HashFull;
                // not exists - can create
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    var existUsers = await dbConnection.QueryAsync<User>("select Id from User where admin = true");
                    var existUser = existUsers.FirstOrDefault();
                    bool admin = userParam.Admin || existUser == null;
                    userParam.Admin = admin;
                    await dbConnection.InsertAsync<User>(userParam);
                }
            }            
        }
        public async Task Delete(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                await dbConnection.DeleteAsync(new User{Id = id});
            }
        }

        public async Task Update(User user, bool isAdmin)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                string addUpdate = isAdmin ? ", Admin = @Admin, HaveAccess = @HaveAccess" : "";
                var count = await dbConnection.ExecuteAsync($"update User set Username = @Username, FirstName = @FirstName, LastName = @LastName, Email = @Email {addUpdate} where Id = @Id", user);
            }
        }

        public async Task UpdatePassword(int id, string password)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                Password pass = new Password(password);
                var count = await dbConnection.ExecuteAsync($"update User set Password = @Password where Id = @Id", new { Id = id , Password = pass.HashFull});
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
