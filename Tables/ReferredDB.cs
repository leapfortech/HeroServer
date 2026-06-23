using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ReferredDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Referred]";

        private static Referred GetReferred(SqlDataReader reader)
        {
            return new Referred(Convert.ToInt64(reader["Id"]),
                                reader["Code"].ToString(),
                                Convert.ToInt64(reader["AppUserId"]),
                                Convert.ToInt64(reader["IdentityId"]),
                                Convert.ToDateTime(reader["CreateDateTime"]),
                                Convert.ToDateTime(reader["UpdateDateTime"]),
                                Convert.ToInt32(reader["Status"]));
        }

        private static ReferredFull GetReferredFull(SqlDataReader reader, bool includeReferrer)
        {
            ReferrerFull referrer = null;

            if (includeReferrer)
                referrer = GetReferrerFull(reader);


            return new ReferredFull(Convert.ToInt64(reader["Id"]),
                                    reader["Code"].ToString(),
                                    Convert.ToInt64(reader["AppUserId"]),
                                    reader["FirstName1"].ToString(),
                                    reader["FirstName2"].ToString(),
                                    reader["LastName1"].ToString(),
                                    reader["LastName2"].ToString(),
                                    reader["PhonePrefix"].ToString(),
                                    reader["Phone"].ToString(),
                                    reader["Email"].ToString(),
                                    Convert.ToDateTime(reader["CreateDateTime"]),
                                    referrer);
        }

        private static ReferrerFull GetReferrerFull(SqlDataReader reader)
        {
            return new ReferrerFull(Convert.ToInt64(reader["ReferrerIdentityId"]),
                                    reader["ReferrerFirstName1"].ToString(),
                                    reader["ReferrerFirstName2"].ToString(),
                                    reader["ReferrerLastName1"].ToString(),
                                    reader["ReferrerLastName2"].ToString(),
                                    reader["ReferrerPhonePrefix"].ToString(),
                                    reader["ReferrerPhone"].ToString(),
                                    reader["ReferrerEmail"].ToString());
        }

        // GET
        public async Task<List<Referred>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Referred> referreds = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Referred referred = GetReferred(reader);
                         referreds.Add(referred);
                    }
                }
            }
            return referreds;
        }

        public async Task<ReferredFullAllRsp> GetFullAllByCode(ReferredAllByCodeReq req)
        {
            req.Page = Math.Max(1, req.Page);
            req.PageSize = Math.Max(1, req.PageSize);

            int offset = (req.Page - 1) * req.PageSize;

            String strCmd = // Count
                            "SELECT COUNT(Referred.Id) AS TotalCount " +
                            "FROM [D-Referred] AS Referred " +
                            "INNER JOIN [D-Identity] IReferred ON IReferred.Id = Referred.IdentityId AND IReferred.Status = 1 " +
                            "INNER JOIN [J-IdentityAppUser] IAU ON IAU.AppUserId = Referred.AppUserId AND IAU.Status = 1 " +
                            "WHERE (@Status = -1 OR Referred.Status = @Status) " +
                            "AND (@Code IS NULL OR Referred.Code LIKE '%' + @Code + '%');" +

                            // Data
                            "SELECT " +
                            " Referred.Id, Referred.Code, Referred.AppUserId, Referred.CreateDateTime, " +

                            " IReferred.Id AS IdentityId, " +
                            " IReferred.FirstName1, IReferred.FirstName2, IReferred.LastName1, IReferred.LastName2, " +
                            " CReferred.PhonePrefix, IReferred.Phone, IReferred.Email, " +

                            " IReferrer.Id AS ReferrerIdentityId, " +
                            " IReferrer.FirstName1 AS ReferrerFirstName1, " +
                            " IReferrer.FirstName2 AS ReferrerFirstName2, " +
                            " IReferrer.LastName1 AS ReferrerLastName1, " +
                            " IReferrer.LastName2 AS ReferrerLastName2, " +
                            " CReferrer.PhonePrefix AS ReferrerPhonePrefix, " +
                            " IReferrer.Phone AS ReferrerPhone, " +
                            " IReferrer.Email AS ReferrerEmail " +

                            "FROM [D-Referred] AS Referred " +
                            "INNER JOIN [D-Identity] IReferred ON IReferred.Id = Referred.IdentityId AND IReferred.Status = 1 " +
                            "INNER JOIN [K-Country] CReferred ON CReferred.Id = IReferred.PhoneCountryId " +

                            "INNER JOIN [J-IdentityAppUser] IAU ON IAU.AppUserId = Referred.AppUserId AND IAU.Status = 1 " +
                            "INNER JOIN [D-Identity] IReferrer ON IReferrer.Id = IAU.IdentityId " +
                            "LEFT JOIN [K-Country] CReferrer ON CReferrer.Id = IReferrer.PhoneCountryId " +

                            "WHERE (@Status = -1 OR Referred.Status = @Status) " +
                            "AND (@Code IS NULL OR Referred.Code LIKE '%' + @Code + '%') " +

                            "ORDER BY Referred.CreateDateTime DESC " +
                            "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Status", SqlDbType.Int, req.Status);
            command.AddParam("@Code", SqlDbType.VarChar, String.IsNullOrWhiteSpace(req.Code) ? DBNull.Value : req.Code);
            command.AddParam("@Offset", SqlDbType.Int, offset);
            command.AddParam("@PageSize", SqlDbType.Int, req.PageSize);

            ReferredFullAllRsp response = null;

            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    // 1. Count
                    int totalCount = 0;
                    if (await reader.ReadAsync())
                        totalCount = Convert.ToInt32(reader["TotalCount"]);

                    int totalPages = (int)Math.Ceiling((double)totalCount / req.PageSize);

                    // 2. Data
                    await reader.NextResultAsync();

                    List<ReferredFull> referredFulls = new List<ReferredFull>();

                    while (await reader.ReadAsync())
                        referredFulls.Add(GetReferredFull(reader, true));

                    response = new ReferredFullAllRsp(req.Page, totalPages, referredFulls);
                }
            }

            return response;
        }

        public async Task<Referred> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            Referred referred = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         referred = GetReferred(reader);
                    }
                }
            }
            return referred;
        }

        public async Task<IEnumerable<Referred>> GetByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);
            command.AddParam("@Status", SqlDbType.Int, status);

            List<Referred> referreds = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Referred referred = GetReferred(reader);
                        referreds.Add(referred);
                    }
                }
            }
            return referreds;
        }

        public async Task<int> GetCountByAppUserId(long appUserId, int status = 1)
        {
            String strCmd = $"SELECT COUNT(1) AS Count FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);
            command.AddParam("@Status", SqlDbType.Int, status);

            int count = -1;
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

        public async Task<IEnumerable<ReferredFull>> GetHistory(long appUserId, DateTime startDate, DateTime endDate)
        {
            String strCmd = @"SELECT referred.Id, referred.Code, referred.AppUserId, idt.FirstName1, idt.FirstName2," +
                             " idt.LastName1, idt.LastName2, idt.PhoneCountryId AS PhonePrefix," +
                             " idt.Phone, idt.Email, referred.CreateDateTime" +
                             " FROM [D-Referred] referred" +
                             " INNER JOIN [D-Identity] idt ON idt.Id = referred.IdentityId" +
                             " WHERE referred.AppUserId = @AppUserId AND referred.Status = 1" +
                             " AND referred.CreateDateTime BETWEEN @DateStart AND @DateEnd" +
                             " ORDER BY referred.CreateDateTime DESC";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);
            command.AddParam("@DateStart", SqlDbType.DateTime2, startDate);
            command.AddParam("@DateEnd", SqlDbType.DateTime2, endDate);

            List<ReferredFull> referredFulls = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ReferredFull referredFull = GetReferredFull(reader, false);
                        referredFulls.Add(referredFull);
                    }
                }
            }
            return referredFulls;
        }

        public async Task<long> GetAppUserIdById(long id)
        {
            String strCmd = $"SELECT AppUserId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            long appUserId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appUserId = Convert.ToInt64(reader["AppUserId"]);
                    }
                }
            }
            return appUserId;
        }

        public async Task<Referred> GetByCode(String code)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Code = @Code";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Code", SqlDbType.VarChar, code);

            Referred referred = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        referred = GetReferred(reader);
                    }
                }
            }
            return referred;
        }

        public async Task<long> GetIdByCode(String code)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE Code = @Code";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Code", SqlDbType.VarChar, code);

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

        public async Task<long> GetAppUserIdByCode(String code)
        {
            String strCmd = $"SELECT AppUserId FROM {table} WHERE Code = @Code";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Code", SqlDbType.VarChar, code);

            int appUserId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        appUserId = Convert.ToInt32(reader["AppUserId"]);
                    }
                }
            }
            return appUserId;
        }

        // INSERT
        public async Task<long> Add(Referred referred)
        {
            String strCmd = $"INSERT INTO {table}(Id, Code, AppUserId, IdentityId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @Code, @AppUserId, @IdentityId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('f'));
            command.AddParam("@Code", SqlDbType.VarChar, referred.Code);
            command.AddParam("@AppUserId", SqlDbType.BigInt, referred.AppUserId);
            command.AddParam("@IdentityId", SqlDbType.BigInt, referred.IdentityId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, referred.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Referred referred)
        {
            String strCmd = $"UPDATE {table} SET Code = @Code, AppUserId = @AppUserId, IdentityId = @IdentityId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Code", SqlDbType.VarChar, referred.Code);
            command.AddParam("@AppUserId", SqlDbType.BigInt, referred.AppUserId);
            command.AddParam("@IdentityId", SqlDbType.BigInt, referred.IdentityId);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@Id", SqlDbType.BigInt, referred.Id);

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

        public async Task<bool> UpdateStatusByAppUserId(long appUserId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE AppUserId = @AppUserId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);

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

        public async Task<bool> DeleteByAppUserId(long appUserId)
        {
            String strCmd = $"DELETE {table} WHERE AppUserId = @AppUserId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@AppUserId", SqlDbType.BigInt, appUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
