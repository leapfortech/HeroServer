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

        private static ReferredFull GetReferredFull(SqlDataReader reader)
        {
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
                                    GetReferrerFull(reader));
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

        public async Task<List<ReferredFull>> GetFullAll()
        {
            String strCmd = "SELECT" +
                            " Referred.Id," +
                            " Referred.Code," +
                            " Referred.AppUserId," +
                            " Referred.CreateDateTime," +

                            " IReferred.Id AS IdentityId," +
                            " IReferred.FirstName1," +
                            " IReferred.FirstName2," +
                            " IReferred.LastName1," +
                            " IReferred.LastName2," +
                            " CReferred.PhonePrefix AS PhonePrefix," +
                            " IReferred.Phone," +
                            " IReferred.Email," +

                            " IReferrer.Id AS ReferrerIdentityId," +
                            " IReferrer.FirstName1 AS ReferrerFirstName1," +
                            " IReferrer.FirstName2 AS ReferrerFirstName2," +
                            " IReferrer.LastName1 AS ReferrerLastName1," +
                            " IReferrer.LastName2 AS ReferrerLastName2," +
                            " CReferrer.PhonePrefix AS ReferrerPhonePrefix," +
                            " IReferrer.Phone AS ReferrerPhone," +
                            " IReferrer.Email AS ReferrerEmail" +

                            " FROM [D-Referred] AS Referred" +

                            " INNER JOIN [D-Identity] IReferred ON IReferred.Id = Referred.IdentityId AND IReferred.Status = 1" +
                            " INNER JOIN [K-Country] CReferred ON CReferred.Id = IReferred.PhoneCountryId " +

                            " INNER JOIN [J-IdentityAppUser] IAU ON IAU.AppUserId = Referred.AppUserId AND IAU.Status = 1" +
                            " INNER JOIN [D-Identity] IReferrer ON IReferrer.Id = IAU.IdentityId" +
                            " INNER JOIN [K-Country] CReferrer ON CReferrer.Id = IReferrer.PhoneCountryId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<ReferredFull> referredFulls = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ReferredFull referredFull = GetReferredFull(reader);
                        referredFulls.Add(referredFull);
                    }
                }
            }
            return referredFulls;
        }

        public async Task<Referred> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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
            String strCmd = @"SELECT referred.Id, referred.Code, idt.FirstName1, idt.FirstName2," +
                             " idt.LastName1, idt.LastName2, idt.PhoneCountryId AS PhonePrefix," +
                             " idt.Phone, idt.Email, referred.CreateDateTime" +
                             " FROM [D-Referred] referred" +
                             " INNER JOIN [D-Identity] idt ON idt.Id = referred.IdentityId" +
                             " WHERE referred.AppUserId = @AppUserId AND referred.Status = 1" +
                             " AND referred.CreateDateTime BETWEEN @DateStart AND @DateEnd" +
                             " ORDER BY referred.CreateDateTime DESC";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@DateStart", SqlDbType.DateTime2, startDate);
            DBHelper.AddParam(command, "@DateEnd", SqlDbType.DateTime2, endDate);

            List<ReferredFull> referredFulls = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ReferredFull referredFull = GetReferredFull(reader);
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

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, code);

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

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, code);

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

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, code);

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

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('f'));
            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, referred.Code);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, referred.AppUserId);
            DBHelper.AddParam(command, "@IdentityId", SqlDbType.BigInt, referred.IdentityId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, referred.Status);

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

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, referred.Code);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, referred.AppUserId);
            DBHelper.AddParam(command, "@IdentityId", SqlDbType.BigInt, referred.IdentityId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, referred.Id);

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

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
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

        public async Task<bool> DeleteByAppUserId(long appUserId)
        {
            String strCmd = $"DELETE {table} WHERE AppUserId = @AppUserId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
