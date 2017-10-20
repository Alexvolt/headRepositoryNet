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
    public class SimpleRepository<T>
    {
        private readonly DataAccessOptions _dataAccessOptions;
        private readonly string _tableName;
        private readonly string _uniqueNameField;
        public SimpleRepository(IOptions<DataAccessOptions> dataAccessOptions, string tableName, string uniqueNameField = "Name")
        {
            _dataAccessOptions = dataAccessOptions.Value;
            _tableName = tableName;
            _uniqueNameField = uniqueNameField;
        }

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(_dataAccessOptions.PostgresConnectionString);
            }
        }

        public async Task<IEnumerable<T>> FindAsync(Dictionary<string, string> queryParams)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                if (queryParams.Count > 0)
                {
                    string query = QueryHelper<T>.BuildSelectQuery(queryParams);
                    return await dbConnection.QueryAsync<T>(query);
                }
                else
                {
                    return await dbConnection.FindAsync<T>();
                }
            }
        }

        public async Task<T> GetByName(string name)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var Simple = await dbConnection.QueryAsync<T>($"select * from \"{_tableName}\" where \"{_uniqueNameField}\" = @Name", new { Name = name });
                return Simple.FirstOrDefault();
            }
        }


        public async Task<T> GetById(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                var Simple = await dbConnection.QueryAsync<T>($"select * from \"{_tableName}\" where \"Id\" = @Id", new { Id = id });
                return Simple.FirstOrDefault();
                //return await dbConnection.GetAsync<T>(new T{ Id = id });
            }
        }

        public async Task Create(T newData)
        {
            if(!String.IsNullOrEmpty(_uniqueNameField))
            {
                string nameValue = typeof(T).GetProperty(_uniqueNameField).GetValue(newData).ToString();
                T existingData = await GetByName(nameValue);
                if (existingData != null)
                {
                    throw new ApplicationException(_uniqueNameField + " already exists");
                }
            }

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                await dbConnection.InsertAsync(newData);
            }

        }

        public async Task Delete(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var count = await dbConnection.ExecuteAsync($"delete from \"{_tableName}\" where \"Id\" = @Id", new { Id = id });
                //var count = await dbConnection.DeleteAsync(new T{ Id = id });

            }
        }

        public async Task Update(T data)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var res = await dbConnection.UpdateAsync(data);
            }
        }

    }
}
