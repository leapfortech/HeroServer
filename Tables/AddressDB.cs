using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class AddressDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Address]";

        public static Address GetAddress(SqlDataReader reader)
        {
            return new Address(Convert.ToInt64(reader["Id"]),
                               Convert.ToInt64(reader["CountryId"]),
                               Convert.ToInt64(reader["StateId"]), 
                               Convert.ToInt64(reader["CityId"]),
                               reader["Address1"].ToString(),
                               reader["Address2"].ToString(), 
                               reader["Zone"].ToString(),
                               reader["ZipCode"].ToString(),
                               reader["Latitude"] == DBNull.Value ? null : (float?)Convert.ToSingle(reader["Latitude"]),
                               reader["Longitude"] == DBNull.Value ? null : (float?)Convert.ToSingle(reader["Longitude"]),
                               Convert.ToDateTime(reader["CreateDateTime"]),
                               Convert.ToDateTime(reader["UpdateDateTime"]),
                               Convert.ToInt32(reader["Status"]));
        }

        public static AddressFull GetAddressFull(SqlDataReader reader)
        {
            return new AddressFull(Convert.ToInt64(reader["Id"]),
                                   reader["Country"].ToString(),
                                   reader["State"].ToString(),
                                   reader["City"].ToString(),
                                   reader["Address1"].ToString(), 
                                   reader["Address2"].ToString(),
                                   reader["Zone"].ToString(),
                                   reader["ZipCode"].ToString(),
                                   reader["Latitude"] == DBNull.Value ? null : (float?)Convert.ToSingle(reader["Latitude"]),
                                   reader["Longitude"] == DBNull.Value ? null : (float?)Convert.ToSingle(reader["Longitude"]),
                                   Convert.ToInt32(reader["Status"]));
        }

        // SELECT
        public async Task<IEnumerable<Address>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Address> addresses = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Address address = GetAddress(reader);
                        addresses.Add(address);
                    }
                }
            }
            return addresses;
        }

        public async Task<Address> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            Address address = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        address = GetAddress(reader);
                    }
                }
            }
            return address;
        }

        // GET ALL
        public async Task<AddressFull> GetAddressFullByAppUserId(long appUserId)
        {
            String strCmd = "SELECT AdrApp.AppUserId AS EntityId, Country.Name AS Country, State.Name AS State, City.Name AS City, Adr.Address1, Adr.Address2, Adr.Zone," +
                            " Adr.ZipCode, Adr.Latitude, Adr.Longitude, AdrApp.Status" + 
                            $" FROM {table} AS Adr INNER JOIN [J-AddressAppUser] AS AdrApp ON (AdrApp.AddressId = Adr.Id)" +
                            " INNER JOIN [K-Country] AS Country ON (Adr.CountryId = Country.Id)" + 
                            " INNER JOIN [K-State] AS State ON (Adr.StateId = State.Id)" +
                            " INNER JOIN [K-City] AS City ON (Adr.CityId = City.Id)" +
                            " WHERE AdrApp.AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);

            AddressFull addressFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        addressFull = GetAddressFull(reader);
                    }
                }
            }
            return addressFull;
        }

        // INSERT
        public async Task<long> Add(Address address)
        {
            String strCmd = $"INSERT INTO {table}(Id, CountryId, StateId, CityId, Address1, Address2, Zone, ZipCode, Latitude, Longitude, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @CountryId, @StateId, @CityId, @Address1, @Address2, @Zone, @ZipCode, @Latitude, @Longitude, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('D'));
            command.AddParam("@CountryId", SqlDbType.BigInt, address.CountryId);
            command.AddParam("@StateId", SqlDbType.BigInt, address.StateId);
            command.AddParam("@CityId", SqlDbType.BigInt, address.CityId);
            command.AddParam("@Address1", SqlDbType.VarChar, address.Address1);
            command.AddParam("@Address2", SqlDbType.VarChar, address.Address2);
            command.AddParam("@Zone", SqlDbType.VarChar, address.Zone);
            command.AddParam("@ZipCode", SqlDbType.VarChar, address.ZipCode);
            command.AddParam("@Latitude", SqlDbType.Decimal, address.Latitude);
            command.AddParam("@Longitude", SqlDbType.Decimal, address.Longitude);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, address.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Address address)
        {
            String strCmd = $"UPDATE {table} SET CountryId = @CountryId, StateId = @StateId, CityId = @CityId, Address1 = @Address1, Address2 = @Address2, Zone = @Zone, ZipCode = @ZipCode," +
                            " Latitude = @Latitude, Longitude = @Longitude, UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@CountryId", SqlDbType.BigInt, address.CountryId);
            command.AddParam("@StateId", SqlDbType.BigInt, address.StateId);
            command.AddParam("@CityId", SqlDbType.BigInt, address.CityId);
            command.AddParam("@Address1", SqlDbType.VarChar, address.Address1);
            command.AddParam("@Address2", SqlDbType.VarChar, address.Address2);
            command.AddParam("@Zone", SqlDbType.VarChar, address.Zone);
            command.AddParam("@ZipCode", SqlDbType.VarChar, address.ZipCode);
            command.AddParam("@Latitude", SqlDbType.Decimal, address.Latitude);
            command.AddParam("@Longitude", SqlDbType.Decimal, address.Longitude);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, address.Status);
            command.AddParam("@Id", SqlDbType.BigInt, address.Id);

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
