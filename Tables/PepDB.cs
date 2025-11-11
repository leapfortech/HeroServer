using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PepDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Pep]";

        private static Pep GetPep(SqlDataReader reader)
        {
            return new Pep(Convert.ToInt32(reader["Id"]),
                           reader["InstitutionName"].ToString(),
                           Convert.ToInt32(reader["InstitutionCountryId"]),
                           reader["JobTitle"].ToString(),
                           Convert.ToInt32(reader["WealthOriginTypeId"]),
                           reader["WealthDescription"].ToString(),
                           Convert.ToDateTime(reader["CreateDatetime"]),
                           Convert.ToDateTime(reader["UpdateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<Pep>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Pep> peps = new List<Pep>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Pep pep = GetPep(reader);
                         peps.Add(pep);
                    }
                }
            }
            return peps;
        }

        public async Task<Pep> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Pep pep = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         pep = GetPep(reader);
                    }
                }
            }
            return pep;
        }

        // INSERT
        public async Task<int> Add(Pep pep)
        {
            String strCmd = $"INSERT INTO {table}(InstitutionName, InstitutionCountryId, JobTitle, WealthOriginTypeId, WealthDescription, CreateDatetime, UpdateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@InstitutionName, @InstitutionCountryId, @JobTitle, @WealthOriginTypeId, @WealthDescription, @CreateDatetime, @UpdateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InstitutionName", SqlDbType.VarChar, pep.InstitutionName);
            DBHelper.AddParam(command, "@InstitutionCountryId", SqlDbType.Int, pep.InstitutionCountryId);
            DBHelper.AddParam(command, "@JobTitle", SqlDbType.VarChar, pep.JobTitle);
            DBHelper.AddParam(command, "@WealthOriginTypeId", SqlDbType.Int, pep.WealthOriginTypeId);
            DBHelper.AddParam(command, "@WealthDescription", SqlDbType.VarChar, pep.WealthDescription);
            DBHelper.AddParam(command, "@CreateDatetime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Pep pep)
        {
            String strCmd = $"UPDATE {table} SET InstitutionName = @InstitutionName, InstitutionCountryId = @InstitutionCountryId, JobTitle = @JobTitle, WealthOriginTypeId = @WealthOriginTypeId, WealthDescription = @WealthDescription, CreateDatetime = @CreateDatetime, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InstitutionName", SqlDbType.VarChar, pep.InstitutionName);
            DBHelper.AddParam(command, "@InstitutionCountryId", SqlDbType.Int, pep.InstitutionCountryId);
            DBHelper.AddParam(command, "@JobTitle", SqlDbType.VarChar, pep.JobTitle);
            DBHelper.AddParam(command, "@WealthOriginTypeId", SqlDbType.Int, pep.WealthOriginTypeId);
            DBHelper.AddParam(command, "@WealthDescription", SqlDbType.VarChar, pep.WealthDescription);
            DBHelper.AddParam(command, "@CreateDatetime", SqlDbType.DateTime2, pep.CreateDatetime);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, pep.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        // DELETE
        public async Task<int> DeleteAll()
        {
            String strCmd = $"DELETE {table}";
            SqlCommand command = new SqlCommand(strCmd, conn);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteById(int id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
