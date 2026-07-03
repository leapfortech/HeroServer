using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ServiceWishDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-ServiceWish]";

        public static ServiceWish GetServiceWish(SqlDataReader reader)
        {
            return new ServiceWish(Convert.ToInt64(reader["Id"]),
                                   Convert.ToInt64(reader["AppUserId"]),
                                   Convert.ToInt64(reader["ServiceTypeId"]),
                                   reader["Wish"].ToString(),
                                   Convert.ToDateTime(reader["CreateDateTime"]),
                                   Convert.ToDateTime(reader["UpdateDateTime"]),
                                   Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<ServiceWish>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<ServiceWish> serviceWishs = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         ServiceWish serviceWish = GetServiceWish(reader);
                         serviceWishs.Add(serviceWish);
                    }
                }
            }
            return serviceWishs;
        }

        public async Task<List<ServiceWish>> GetAllByStatus(int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Status", SqlDbType.BigInt, status);

            List<ServiceWish> serviceWishs = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ServiceWish serviceWish = GetServiceWish(reader);
                        serviceWishs.Add(serviceWish);
                    }
                }
            }
            return serviceWishs;
        }

        public async Task<ServiceWishAllRsp> GetAllByType(ServiceWishAllByTypeReq req)
        {
            int offset = (req.Page - 1) * req.PageSize;

            String strCmd = // Total count
                            "SELECT COUNT(SW.Id) AS TotalCount " +
                            "FROM [D-ServiceWish] AS SW " +
                            "WHERE (@Status = -1 OR SW.Status = @Status) " +
                            "AND (@ServiceWishTypeId = -1 OR SW.ServiceTypeId = @ServiceWishTypeId);" +

                            // Data
                            "SELECT " +
                            "SW.Id, SW.AppUserId, SW.ServiceTypeId, SW.Wish, " +
                            "SW.CreateDateTime, SW.UpdateDateTime, SW.Status, " +

                            // WebSysUser
                            "WSU.Email, WSU.PhoneCountryId, WSU.Phone, " +

                            // Identity
                            "I.FirstName1, I.FirstName2, " +
                            "I.LastName1, I.LastName2, " +
                            "I.GenderId, I.BirthDate, " +
                            "I.BirthCountryId, I.BirthStateId, I.BirthCityId, " +

                            // Address
                            "A.CountryId, A.StateId, A.CityId " +

                            "FROM [D-ServiceWish] AS SW " +
                            "LEFT JOIN [D-AppUser] AS AU ON AU.Id = SW.AppUserId " +
                            "LEFT JOIN [D-WebSysUser] AS WSU ON WSU.Id = AU.WebSysUserId " +
                            "LEFT JOIN [J-IdentityAppUser] AS IAU " +
                            "ON IAU.AppUserId = AU.Id AND IAU.Status = 1 " +
                            "LEFT JOIN [D-Identity] AS I ON I.Id = IAU.IdentityId " +
                            "LEFT JOIN [J-AddressAppUser] AS AAU " +
                            "ON AAU.AppUserId = AU.Id AND AAU.Status = 1 " +
                            "LEFT JOIN [D-Address] AS A ON A.Id = AAU.AddressId " +

                            "WHERE (@Status = -1 OR SW.Status = @Status) " +
                            "AND (@ServiceWishTypeId = -1 OR SW.ServiceTypeId = @ServiceWishTypeId) " +

                            "ORDER BY SW.CreateDateTime DESC " +
                            "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Status", SqlDbType.Int, req.Status);
            command.AddParam("@ServiceWishTypeId", SqlDbType.BigInt, req.ServiceWishTypeId);
            command.AddParam("@Offset", SqlDbType.Int, offset);
            command.AddParam("@PageSize", SqlDbType.Int, req.PageSize);

            ServiceWishAllRsp response = null;

            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    // 1. Total count
                    int totalCount = 0;
                    if (await reader.ReadAsync())
                        totalCount = Convert.ToInt32(reader["TotalCount"]);

                    int totalPages = (int)Math.Ceiling((double)totalCount / req.PageSize);

                    // 2. Data
                    await reader.NextResultAsync();

                    List<ServiceWishInfo> serviceWishInfos = new List<ServiceWishInfo>();

                    while (await reader.ReadAsync())
                    {
                        ServiceWish serviceWish = GetServiceWish(reader);

                        ServiceWishUser serviceWishUser = new ServiceWishUser(
                            serviceWish,
                            reader["PhoneCountryId"] == DBNull.Value ? -1 : Convert.ToInt64(reader["PhoneCountryId"]),
                            reader["Phone"] == DBNull.Value ? "" : reader["Phone"].ToString(),
                            reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString(),
                            reader["FirstName1"] == DBNull.Value ? "" : reader["FirstName1"].ToString(),
                            reader["FirstName2"] == DBNull.Value ? "" : reader["FirstName2"].ToString(),
                            reader["LastName1"] == DBNull.Value ? "" : reader["LastName1"].ToString(),
                            reader["LastName2"] == DBNull.Value ? "" : reader["LastName2"].ToString(),
                            reader["GenderId"] == DBNull.Value ? -1 : Convert.ToInt64(reader["GenderId"]),
                            reader["BirthDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["BirthDate"]),
                            reader["BirthCountryId"] == DBNull.Value ? -1 : Convert.ToInt64(reader["BirthCountryId"]),
                            reader["BirthStateId"] == DBNull.Value ? -1 : Convert.ToInt64(reader["BirthStateId"]),
                            reader["BirthCityId"] == DBNull.Value ? -1 : Convert.ToInt64(reader["BirthCityId"]),
                            reader["CountryId"] == DBNull.Value ? -1 : Convert.ToInt64(reader["CountryId"]),
                            reader["StateId"] == DBNull.Value ? -1 : Convert.ToInt64(reader["StateId"]),
                            reader["CityId"] == DBNull.Value ? -1 : Convert.ToInt64(reader["CityId"]));

                        serviceWishInfos.Add(new ServiceWishInfo(serviceWish, serviceWishUser));
                    }

                    response = new ServiceWishAllRsp(req.Page, totalPages, serviceWishInfos);
                }
            }

            return response;
        }

        public async Task<ServiceWish> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            ServiceWish serviceWish = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         serviceWish = GetServiceWish(reader);
                    }
                }
            }
            return serviceWish;
        }

        // INSERT
        public async Task<long> Add(ServiceWish serviceWish)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, ServiceTypeId, Wish, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @ServiceTypeId, @Wish, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, serviceWish.AppUserId);
            command.AddParam("@ServiceTypeId", SqlDbType.BigInt, serviceWish.ServiceTypeId);
            command.AddParam("@Wish", SqlDbType.VarChar, serviceWish.Wish);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, serviceWish.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(ServiceWish serviceWish)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, ServiceTypeId = @ServiceTypeId, Wish = @Wish, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, serviceWish.AppUserId);
            command.AddParam("@ServiceTypeId", SqlDbType.BigInt, serviceWish.ServiceTypeId);
            command.AddParam("@Wish", SqlDbType.VarChar, serviceWish.Wish);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Id", SqlDbType.BigInt, serviceWish.Id);

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

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);
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
