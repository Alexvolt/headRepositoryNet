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
    public class ProfessionalAreasRepository: SimpleRepository<ProfessionalArea>
    {
        public ProfessionalAreasRepository(IOptions<DataAccessOptions> dataAccessOptions) 
            : base(dataAccessOptions, "ProfessionalAreas", "Name")
        {
        }

        public async Task<IEnumerable<NameWithId>> GetParents()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                return await dbConnection.QueryAsync<NameWithId>("select \"Id\", \"Name\" from \"ProfessionalAreas\" where \"ParentId\" is null order by \"Name\"");
            }
        }
    }
}
