using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class LocalityDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Locality]";

        public static Locality GetLocality(SqlDataReader reader)
        {
            return new Locality(Convert.ToInt64(reader["Id"]),
                               Convert.ToInt64(reader["AppUserId"]),
                               Convert.ToInt32(reader["LocalityType"]),
                               Convert.ToInt64(reader["CountryId"]),
                               Convert.ToInt64(reader["StateId"]),
                               Convert.ToInt64(reader["CityId"]),
                               Convert.ToDateTime(reader["CreateDateTime"]),
                               Convert.ToDateTime(reader["UpdateDateTime"]),
                               Convert.ToInt32(reader["Status"]));
        }

        // SELECT
        public async Task<IEnumerable<Locality>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Locality> Localitys = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Locality locality = GetLocality(reader);
                        Localitys.Add(locality);
                    }
                }
            }
            return Localitys;
        }

        public async Task<Locality> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            Locality locality = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        locality = GetLocality(reader);
                    }
                }
            }
            return locality;
        }

        public async Task<IEnumerable<Locality>> GetByAppUserId(long appUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);

            List<Locality> Localitys = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Locality locality = GetLocality(reader);
                        Localitys.Add(locality);
                    }
                }
            }
            return Localitys;
        }

        public async Task<Locality> GetByAppUserIdAndLocalityType(long appUserId, int localityType)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND LocalityType = @LocalityType";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);
            command.AddParam("@LocalityType", SqlDbType.Int, localityType);

            Locality locality = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        locality = GetLocality(reader);
                    }
                }
            }
            return locality;
        }

        // INSERT
        public async Task<long> Add(Locality locality)
        {
            String strCmd = $"INSERT INTO {table}(Id, AppUserId, LocalityType, CountryId, StateId, CityId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @AppUserId, @LocalityType, @CountryId, @StateId, @CityId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('L'));
            command.AddParam("@AppUserId", SqlDbType.BigInt, locality.AppUserId);
            command.AddParam("@LocalityType", SqlDbType.Int, locality.LocalityType);
            command.AddParam("@CountryId", SqlDbType.BigInt, locality.CountryId);
            command.AddParam("@StateId", SqlDbType.BigInt, locality.StateId);
            command.AddParam("@CityId", SqlDbType.BigInt, locality.CityId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, locality.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Locality locality)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, LocalityType = @LocalityType, CountryId = @CountryId, StateId = @StateId, CityId = @CityId" +
                            " UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('L'));
            command.AddParam("@AppUserId", SqlDbType.BigInt, locality.AppUserId);
            command.AddParam("@LocalityType", SqlDbType.Int, locality.LocalityType);
            command.AddParam("@CountryId", SqlDbType.BigInt, locality.CountryId);
            command.AddParam("@StateId", SqlDbType.BigInt, locality.StateId);
            command.AddParam("@CityId", SqlDbType.BigInt, locality.CityId);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, locality.Status);
            command.AddParam("@Id", SqlDbType.BigInt, locality.Id);

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

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE Id = @Id AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByAppUserId(long appUserId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE AppUserId = @AppUserId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);
            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);

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

            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
