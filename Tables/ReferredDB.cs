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
        readonly String table = "[DD-Referred]";

        private static Referred GetReferred(SqlDataReader reader)
        {
            return new Referred(Convert.ToInt32(reader["Id"]),
                                reader["Code"].ToString(),
                                Convert.ToInt32(reader["AppUserId"]),
                                Convert.ToInt32(reader["ProductId"]),
                                reader["FirstName"].ToString(),
                                reader["LastName"].ToString(),
                                Convert.ToInt32(reader["PhoneCountryId"]),
                                reader["Phone"].ToString(),
                                reader["Email"].ToString(),
                                Convert.ToDateTime(reader["CreateDateTime"]),
                                Convert.ToDateTime(reader["UpdateDateTime"]),
                                Convert.ToInt32(reader["Status"]));
        }

        private static ReferredFull GetReferredFull(SqlDataReader reader)
        {
            return new ReferredFull(Convert.ToInt32(reader["Id"]),
                                    reader["Code"].ToString(),
                                    Convert.ToInt32(reader["AppUserId"]),
                                    reader["FirstName"].ToString(),
                                    reader["LastName"].ToString(),
                                    reader["PhonePrefix"].ToString(),
                                    reader["Phone"].ToString(),
                                    reader["Email"].ToString(),
                                    Convert.ToDateTime(reader["CreateDateTime"]),
                                    GetReferrerFull(reader));
        }

        private static ReferrerFull GetReferrerFull(SqlDataReader reader)
        {
            return new ReferrerFull(Convert.ToInt32(reader["IdentityId"]),
                                    reader["Cui"].ToString(),
                                    reader["FirstName1"].ToString(),
                                    reader["FirstName2"].ToString(),
                                    reader["LastName1"].ToString(),
                                    reader["LastName2"].ToString(),
                                    reader["IPhonePrefix"].ToString(),
                                    reader["IPhone"].ToString(),
                                    reader["IEmail"].ToString());
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
            String strCmd = "SELECT Referred.Id, Referred.Code, Referred.AppUserId, Referred.Firstname, Referred.LastName, RCountry.PhonePrefix, Referred.Phone, Referred.Email, Referred.CreateDateTime," +
                            " Identty.Id AS IdentityId, Identty.DpiCui AS Cui, Identty.FirstName1, Identty.FirstName2, Identty.LastName1, Identty.LastName2, ICountry.PhonePrefix AS IPhonePrefix," +
                            $" Identty.Phone AS IPhone, Identty.Email AS IEmail FROM {table} AS Referred" +
                            " INNER JOIN [DD-Identity] AS Identty ON Referred.AppUserId = Identty.AppUserId AND Identty.Status = 1" +
                            " INNER JOIN [K-Country] AS RCountry ON Identty.PhoneCountryId = RCountry.Id" +
                            " INNER JOIN [K-Country] AS ICountry ON Identty.PhoneCountryId = ICountry.Id";

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

        public async Task<Referred> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

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

        public async Task<IEnumerable<Referred>> GetByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<int> GetCountByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT COUNT(1) AS Count FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<IEnumerable<Referred>> GetHistory(int appUserId, DateTime startDate, DateTime endDate)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = 1 AND CreateDateTime BETWEEN @DateStart AND @DateEnd ORDER BY CreateDatetime DESC";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@DateStart", SqlDbType.DateTime2, startDate);
            DBHelper.AddParam(command, "@DateEnd", SqlDbType.DateTime2, endDate);

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

        public async Task<int> GetIdByCode(String code)
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

        public async Task<int> GetAppUserIdByCode(String code)
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
        public async Task<int> Add(Referred referred)
        {
            String strCmd = $"INSERT INTO {table}(Code, AppUserId, ProductId, FirstName, LastName, PhoneCountryId, Phone, Email, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Code, @AppUserId, @ProductId, @FirstName, @LastName, @PhoneCountryId, @Phone, @Email, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, referred.Code);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, referred.AppUserId);
            DBHelper.AddParam(command, "@ProductId", SqlDbType.Int, referred.ProductId);
            DBHelper.AddParam(command, "@FirstName", SqlDbType.VarChar, referred.FirstName);
            DBHelper.AddParam(command, "@LastName", SqlDbType.VarChar, referred.LastName);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, referred.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, referred.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, referred.Email);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, referred.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Referred referred)
        {
            String strCmd = $"UPDATE {table} SET Code = @Code, AppUserId = @AppUserId, ProductId = @ProductId, FirstName = @FirstName, LastName = @LastName, PhoneCountryId = @PhoneCountryId, Phone = @Phone, Email = @Email, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Code", SqlDbType.VarChar, referred.Code);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, referred.AppUserId);
            DBHelper.AddParam(command, "@ProductId", SqlDbType.Int, referred.ProductId);
            DBHelper.AddParam(command, "@FirstName", SqlDbType.VarChar, referred.FirstName);
            DBHelper.AddParam(command, "@LastName", SqlDbType.VarChar, referred.LastName);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, referred.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, referred.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, referred.Email);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, referred.Id);

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

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByAppUserId(int appUserId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE AppUserId = @AppUserId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
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

        public async Task<bool> DeleteByAppUserId(int appUserId)
        {
            String strCmd = $"DELETE {table} WHERE AppUserId = @AppUserId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
