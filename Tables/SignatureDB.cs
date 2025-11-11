using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class SignatureDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Signature]";

        private static Signature GetSignature(SqlDataReader reader)
        {
            return new Signature(Convert.ToInt32(reader["Id"]),
                                 Convert.ToInt32(reader["AppUserId"]),
                                 reader["Strokes"].ToString(),
                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                 Convert.ToInt32(reader["Status"]));
        }

        // SELECT
        public async Task<IEnumerable<Signature>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Signature> signatures = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Signature signature = GetSignature(reader);
                         signatures.Add(signature);
                    }
                }
            }
            return signatures;
        }

        public async Task<Signature> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Signature signature = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         signature = GetSignature(reader);
                    }
                }
            }
            return signature;
        }

        // INSERT
        public async Task<int> Add(Signature signature)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, Strokes, CreateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @Strokes, @CreateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, signature.AppUserId);
            DBHelper.AddParam(command, "@Strokes", SqlDbType.VarChar, signature.Strokes);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, signature.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Signature signature)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, Strokes = @Strokes WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, signature.AppUserId);
            DBHelper.AddParam(command, "@Strokes", SqlDbType.VarChar, signature.Strokes);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, signature.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> SetStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

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
