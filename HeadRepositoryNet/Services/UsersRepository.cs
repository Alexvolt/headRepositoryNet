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

                var users = await dbConnection.QueryAsync<User>("select * from \"Users\" where \"Username\" = @Username", new { Username = userName });
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
                    var existUsers = await dbConnection.QueryAsync<User>("select \"Id\" from \"Users\" where \"Admin\" = true");
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
                var count = await dbConnection.ExecuteAsync($"update \"Users\" set \"Username\" = @Username, \"FirstName\" = @FirstName, \"LastName\" = @LastName, \"Email\" = @Email {addUpdate} where \"Id\" = @Id", user);
            }
        }

        public async Task UpdatePassword(int id, string password)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                Password pass = new Password(password);
                var count = await dbConnection.ExecuteAsync($"update \"Users\" set \"Password\" = @Password where \"Id\" = @Id", new { Id = id , Password = pass.HashFull});
            }
        }

    

    }
}
