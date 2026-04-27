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
                                   reader["Comment"].ToString(),
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

            DBHelper.AddParam(command, "@Status", SqlDbType.BigInt, status);

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

            String strCmd =// Total count
                            "SELECT COUNT(SW.Id) AS TotalCount " +
                            "FROM [D-ServiceWish] AS SW " +
                            "WHERE (@Status = -1 OR SW.Status = @Status) " +
                            "AND (@ServiceWishTypeId = -1 OR SW.ServiceTypeId = @ServiceWishTypeId);" +

                            // Data
                            "SELECT SW.Id, SW.AppUserId, SW.ServiceTypeId, SW.Comment, " +
                            "SW.CreateDateTime, SW.UpdateDateTime, SW.Status " +
                            "FROM [D-ServiceWish] AS SW " +
                            "WHERE (@Status = -1 OR SW.Status = @Status) " +
                            "AND (@ServiceWishTypeId = -1 OR SW.ServiceTypeId = @ServiceWishTypeId) " +
                            "ORDER BY SW.CreateDateTime DESC " +
                            "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, req.Status);
            DBHelper.AddParam(command, "@ServiceWishTypeId", SqlDbType.BigInt, req.ServiceWishTypeId);
            DBHelper.AddParam(command, "@Offset", SqlDbType.Int, offset);
            DBHelper.AddParam(command, "@PageSize", SqlDbType.Int, req.PageSize);

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

                    List<ServiceWish> serviceWishs = new List<ServiceWish>();

                    while (await reader.ReadAsync())
                        serviceWishs.Add(GetServiceWish(reader));

                    response = new ServiceWishAllRsp(req.Page, totalPages, serviceWishs);
                }
            }

            return response;
        }

        public async Task<ServiceWish> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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
            String strCmd = $"INSERT INTO {table}(AppUserId, ServiceTypeId, Comment, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @ServiceTypeId, @Comment, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, serviceWish.AppUserId);
            DBHelper.AddParam(command, "@ServiceTypeId", SqlDbType.BigInt, serviceWish.ServiceTypeId);
            DBHelper.AddParam(command, "@Comment", SqlDbType.VarChar, serviceWish.Comment);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, serviceWish.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(ServiceWish serviceWish)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, ServiceTypeId = @ServiceTypeId, Comment = @Comment, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, serviceWish.AppUserId);
            DBHelper.AddParam(command, "@ServiceTypeId", SqlDbType.BigInt, serviceWish.ServiceTypeId);
            DBHelper.AddParam(command, "@Comment", SqlDbType.VarChar, serviceWish.Comment);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, serviceWish.Id);

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
