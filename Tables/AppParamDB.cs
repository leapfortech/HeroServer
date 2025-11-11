using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class AppParamDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[S-AppParam]";

        // SELECT
        public async Task<IEnumerable<AppParam>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<AppParam> appParams = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        AppParam appParam = new AppParam(Convert.ToInt32(reader["Id"]), reader["Name"].ToString(), reader["Value"].ToString());
                        appParams.Add(appParam);
                    }
                }
            }

            return appParams;
        }

        public async Task<AppParam> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            AppParam appParam = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appParam = new AppParam(Convert.ToInt32(reader["Id"]), reader["Name"].ToString(), reader["Value"].ToString());
                    }
                }
            }

            return appParam;
        }

        public async Task<String> GetValue(int id)
        {
            String strCmd = $"SELECT Value FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            String value = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        value = reader["Value"].ToString();
                    }
                }
            }

            return value;
        }

        public async Task<String> GetValue(String key)
        {
            String strCmd = $"SELECT Value FROM {table} WHERE Name = @Name";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, key);

            String value = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        value = reader["Value"].ToString();
                    }
                }
            }

            return value;
        }


        // UPDATE
        public async Task<bool> Update(AppParam appParam)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Name = @Name," +
                            " Value = @Value" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, appParam.Name);
            DBHelper.AddParam(command, "@Value", SqlDbType.VarChar, appParam.Value);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, appParam.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> SetValue(String key, String value)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Value = @Value" +
                            " WHERE Name = @Name";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Value", SqlDbType.VarChar, value);
            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, key);

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

        public async Task<bool> DeleteById(int id)
        {
            String strCmd = $"DELETE FROM {table} WHERE Id = @Id";

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
