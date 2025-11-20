using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class HappeningDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Happening]";

        private static Happening GetHappening(SqlDataReader reader)
        {
            return new Happening(Convert.ToInt64(reader["Id"]),
                                 Convert.ToInt64(reader["PostId"]),
                                 Convert.ToInt64(reader["EventTypeId"]),
                                 Convert.ToInt64(reader["CountryId"]),
                                 Convert.ToInt64(reader["StateId"]),
                                 reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["StartDateTime"]),
                                 reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EndDateTime"]),
                                 reader["Location"].ToString(),
                                 reader["Latitude"] == DBNull.Value ? null : (double?)Convert.ToDouble(reader["Latitude"]),
                                 reader["Longitude"] == DBNull.Value ? null : (double?)Convert.ToDouble(reader["Longitude"]),
                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                 Convert.ToDateTime(reader["UpdateDateTime"]),
                                 Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Happening>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Happening> happenings = new List<Happening>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Happening happening = GetHappening(reader);
                         happenings.Add(happening);
                    }
                }
            }
            return happenings;
        }

        public async Task<Happening> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Happening happening = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         happening = GetHappening(reader);
                    }
                }
            }
            return happening;
        }

        // INSERT
        public async Task<long> Add(Happening happening)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, EventTypeId, CountryId, StateId, StartDateTime, EndDateTime, Location, Latitude, Longitude, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @EventTypeId, @CountryId, @StateId, @StartDateTime, @EndDateTime, @Location, @Latitude, @Longitude, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, happening.PostId);
            DBHelper.AddParam(command, "@EventTypeId", SqlDbType.BigInt, happening.EventTypeId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, happening.CountryId);
            DBHelper.AddParam(command, "@StateId", SqlDbType.BigInt, happening.StateId);
            DBHelper.AddParam(command, "@StartDateTime", SqlDbType.DateTime, happening.StartDateTime);
            DBHelper.AddParam(command, "@EndDateTime", SqlDbType.DateTime, happening.EndDateTime);
            DBHelper.AddParam(command, "@Location", SqlDbType.VarChar, happening.Location);
            DBHelper.AddParam(command, "@Latitude", SqlDbType.Decimal, happening.Latitude);
            DBHelper.AddParam(command, "@Longitude", SqlDbType.Decimal, happening.Longitude);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, happening.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Happening happening)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, EventTypeId = @EventTypeId, CountryId = @CountryId, StateId = @StateId, StartDateTime = @StartDateTime, EndDateTime = @EndDateTime, Location = @Location, Latitude = @Latitude, Longitude = @Longitude, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, happening.PostId);
            DBHelper.AddParam(command, "@EventTypeId", SqlDbType.BigInt, happening.EventTypeId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, happening.CountryId);
            DBHelper.AddParam(command, "@StateId", SqlDbType.BigInt, happening.StateId);
            DBHelper.AddParam(command, "@StartDateTime", SqlDbType.DateTime, happening.StartDateTime);
            DBHelper.AddParam(command, "@EndDateTime", SqlDbType.DateTime, happening.EndDateTime);
            DBHelper.AddParam(command, "@Location", SqlDbType.VarChar, happening.Location);
            DBHelper.AddParam(command, "@Latitude", SqlDbType.Decimal, happening.Latitude);
            DBHelper.AddParam(command, "@Longitude", SqlDbType.Decimal, happening.Longitude);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, happening.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, happening.Id);

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
