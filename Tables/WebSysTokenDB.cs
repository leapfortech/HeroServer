using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class WebSysTokenDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-WebSysToken]";

        public static WebSysToken GetWebSysToken(SqlDataReader reader)
        {
            return new WebSysToken(Convert.ToInt64(reader["Id"]), Convert.ToInt64(reader["WebSysUserId"]), reader["Token"].ToString(),
                                   Convert.ToDateTime(reader["CreateDateTime"]), Convert.ToDateTime(reader["UpdateDateTime"]),
                                   Convert.ToInt32(reader["Status"]));
        }

        // SELECT
        public async Task<List<WebSysToken>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<WebSysToken> webSysTokens = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        WebSysToken webSysToken = GetWebSysToken(reader);
                        webSysTokens.Add(webSysToken);
                    }
                }
            }

            return webSysTokens;
        }

        public async Task<WebSysToken> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            WebSysToken webSysToken = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        webSysToken = GetWebSysToken(reader);
                    }
                }
            }

            return webSysToken;
        }

        public async Task<List<WebSysToken>> GetByWebSysUserId(long webSysUserId, int status = -1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE WebSysUserId = @WebSysUserId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysUserId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<WebSysToken> webSysTokens = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        WebSysToken webSysToken = GetWebSysToken(reader);
                        webSysTokens.Add(webSysToken);
                    }
                }
            }

            return webSysTokens;
        }

        public async Task<int> Find(WebSysToken webSysToken)
        {
            String strCmd = $"SELECT * FROM {table} WHERE WebSysUserId = @WebSysUserId AND Token = @Token AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysToken.WebSysUserId);
            DBHelper.AddParam(command, "@Token", SqlDbType.VarChar, webSysToken.Token);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, webSysToken.Status);

            int id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt32(reader["Id"]);
                    }
                }
            }

            return id;
        }

        // INSERT
        public async Task<long> Add(WebSysToken webSysToken)
        {
            String strCmd = $"INSERT INTO {table} (Id, WebSysUserId, Token, CreateDateTime, UpdateDateTime, Status)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @WebSysUserId, @Token, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid());
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysToken.WebSysUserId);
            DBHelper.AddParam(command, "@Token", SqlDbType.VarChar, webSysToken.Token);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, webSysToken.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(WebSysToken webSysToken)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET WebSysUserId = @WebSysUserId, Token = @Token, UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysToken.WebSysUserId);
            DBHelper.AddParam(command, "@Token", SqlDbType.VarChar, webSysToken.Token);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, webSysToken.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Status = @Status, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
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
            String strCmd = $"DELETE FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteById(long id)
        {
            String strCmd = $"DELETE FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<int> DeleteByWebSysUserId(long webSysUserId)
        {
            String strCmd = $"DELETE FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}
