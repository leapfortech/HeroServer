using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class TreatmentDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Treatment]";

        private static Treatment GetTreatment(SqlDataReader reader)
        {
            return new Treatment(Convert.ToInt64(reader["Id"]),
                                 Convert.ToInt64(reader["PostId"]),
                                 Convert.ToInt64(reader["RecipeTypeId"]),
                                 reader["Ingredients"].ToString(),
                                 reader["Preparation"].ToString(),
                                 reader["Usage"].ToString(),
                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                 Convert.ToDateTime(reader["UpdateDateTime"]),
                                 Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Treatment>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Treatment> treatments = new List<Treatment>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Treatment treatment = GetTreatment(reader);
                         treatments.Add(treatment);
                    }
                }
            }
            return treatments;
        }

        public async Task<Treatment> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Treatment treatment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         treatment = GetTreatment(reader);
                    }
                }
            }
            return treatment;
        }

        // INSERT
        public async Task<long> Add(Treatment treatment)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, RecipeTypeId, Ingredients, Preparation, Usage, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @RecipeTypeId, @Ingredients, @Preparation, @Usage, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, treatment.PostId);
            DBHelper.AddParam(command, "@RecipeTypeId", SqlDbType.BigInt, treatment.RecipeTypeId);
            DBHelper.AddParam(command, "@Ingredients", SqlDbType.VarChar, treatment.Ingredients);
            DBHelper.AddParam(command, "@Preparation", SqlDbType.VarChar, treatment.Preparation);
            DBHelper.AddParam(command, "@Usage", SqlDbType.VarChar, treatment.Usage);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, treatment.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Treatment treatment)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, RecipeTypeId = @RecipeTypeId, Ingredients = @Ingredients, Preparation = @Preparation, Usage = @Usage, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, treatment.PostId);
            DBHelper.AddParam(command, "@RecipeTypeId", SqlDbType.BigInt, treatment.RecipeTypeId);
            DBHelper.AddParam(command, "@Ingredients", SqlDbType.VarChar, treatment.Ingredients);
            DBHelper.AddParam(command, "@Preparation", SqlDbType.VarChar, treatment.Preparation);
            DBHelper.AddParam(command, "@Usage", SqlDbType.VarChar, treatment.Usage);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, treatment.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, treatment.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

        public async Task<bool> DeleteById(long id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
