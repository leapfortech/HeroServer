using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class LeapUsageDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-LeapUsage]";

        private static LeapUsage GetLeapUsage(SqlDataReader reader)
        {
            return new LeapUsage(
                             Convert.ToInt32(reader["Id"]),
                             Convert.ToInt32(reader["AppUserId"]),
                             Convert.ToInt32(reader["ProductId"]),
                             Convert.ToInt32(reader["ResponseCode"]),
                             reader["ResponseMessage"].ToString(),
                             Convert.ToDateTime(reader["CreateDateTime"]),
                             Convert.ToDateTime(reader["UpdateDateTime"]),
                             Convert.ToInt32(reader["Status"])
             );
        }

        // GET
        public async Task<IEnumerable<LeapUsage>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<LeapUsage> leapUsages = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         LeapUsage leapUsage = GetLeapUsage(reader);
                         leapUsages.Add(leapUsage);
                    }
                }
            }
            return leapUsages;
        }

        public async Task<LeapUsage> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            LeapUsage leapUsage = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         leapUsage = GetLeapUsage(reader);
                    }
                }
            }
            return leapUsage;
        }

        public async Task<List<LeapUsage>> GetByAppUserId(int appUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            List<LeapUsage> leapUsages = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        LeapUsage leapUsage = GetLeapUsage(reader);
                        leapUsages.Add(leapUsage);
                    }
                }
            }
            return leapUsages;
        }

        public async Task<int> GetCountByAppUserId(int appUserId)
        {
            String strCmd = $"SELECT COUNT(Id) Count FROM {table} WHERE AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            int count = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        count = Convert.ToInt32(reader["Count"]);
                    }
                }
            }
            return count;
        }

        public async Task<int> GetCountByProductId(int appUserId, int productId, int status)
        {
            String strCmd = $"SELECT COUNT(Id) Count FROM {table} WHERE AppUserId = @AppUserId AND ProductId = @ProductId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@ProductId", SqlDbType.Int, productId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            int count = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        count = Convert.ToInt32(reader["Count"]);
                    }
                }
            }
            return count;
        }

        // INSERT
        public async Task<int> Add(LeapUsage leapUsage)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, ProductId, ResponseCode, ResponseMessage, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @ProductId, @ResponseCode, @ResponseMessage, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, leapUsage.AppUserId);
            DBHelper.AddParam(command, "@ProductId", SqlDbType.Int, leapUsage.ProductId);
            DBHelper.AddParam(command, "@ResponseCode", SqlDbType.Int, leapUsage.ResponseCode);
            DBHelper.AddParam(command, "@ResponseMessage", SqlDbType.VarChar, leapUsage.ResponseMessage);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, leapUsage.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(LeapUsage leapUsage)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, ProductId = @ProductId, ResponseCode = @ResponseCode, ResponseMessage = @ResponseMessage, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, leapUsage.AppUserId);
            DBHelper.AddParam(command, "@ProductId", SqlDbType.Int, leapUsage.ProductId);
            DBHelper.AddParam(command, "@ResponseCode", SqlDbType.Int, leapUsage.ResponseCode);
            DBHelper.AddParam(command, "@ResponseMessage", SqlDbType.VarChar, leapUsage.ResponseMessage);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, leapUsage.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByAppUserId(int appUserId, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByProductId(int appUserId, int productId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE AppUserId = @AppUserId AND ProductId = @ProductId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@ProductId", SqlDbType.Int, productId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);

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
