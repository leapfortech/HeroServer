using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class WebSysUserDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-WebSysUser]";

        public static WebSysUser GetWebSysUser(SqlDataReader reader)
        {
            return new WebSysUser(Convert.ToInt32(reader["Id"]),
                                  reader["AuthUserId"].ToString(),
                                  reader["Email"].ToString(),
                                  reader["Roles"].ToString(),
                                  Convert.ToInt32(reader["PhoneCountryId"]),
                                  reader["Phone"].ToString(),
                                  reader["Pin"].ToString(),
                                  Convert.ToInt32(reader["PinFails"]),
                                  reader["PinDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["PinDateTime"]),
                                  Convert.ToDateTime(reader["CreateDateTime"]),
                                  Convert.ToDateTime(reader["UpdateDateTime"]),
                                  Convert.ToInt32(reader["WebSysUserStatusId"]));
        }

        // SELECT
        public async Task<List<WebSysUser>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<WebSysUser> webSysUsers = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        WebSysUser webSysUser = GetWebSysUser(reader);
                        webSysUsers.Add(webSysUser);
                    }
                }
            }

            return webSysUsers;
        }

        public async Task<WebSysUser> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            WebSysUser webSysUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        webSysUser = GetWebSysUser(reader);
                    }
                }
            }

            return webSysUser;
        }

        public async Task<String> GetRoles(int id)
        {
            String strCmd = $"SELECT Roles FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            String roles = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        roles = reader["Roles"].ToString();
                    }
                }
            }

            return roles;
        }

        public async Task<WebSysUser> GetByAuthUserId(String authUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AuthUserId = @AuthUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AuthUserId", SqlDbType.VarChar, authUserId);

            WebSysUser webSysUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        webSysUser = GetWebSysUser(reader);
                    }
                }
            }

            return webSysUser;
        }

        public async Task<int> GetIdByAuthUserId(String authUserId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE AuthUserId = @AuthUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AuthUserId", SqlDbType.VarChar, authUserId);

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

        public async Task<String> GetAuthUserIdById(int id)
        {
            String strCmd = $"SELECT AuthUserId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            String authUserId = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        authUserId = reader["AuthUserId"].ToString();
                    }
                }
            }

            return authUserId;
        }

        public async Task<WebSysUser> GetByEmail(String eMail)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Email = @Email";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, eMail);

            WebSysUser webSysUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        webSysUser = GetWebSysUser(reader);
                    }
                }
            }

            return webSysUser;
        }

        public async Task<int> GetIdByEmail(String eMail)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE Email = @Email";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, eMail);

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

        public async Task<String> GetEmailById(int id)
        {
            String strCmd = $"SELECT Email FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            String email = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        email = reader["Email"].ToString();
                    }
                }
            }

            return email;
        }

        //public async Task<String> GetEmailByAuthUserId(String authUserId)
        //{
        //    String strCmd = $"SELECT Email FROM {table} WHERE AuthUserId = @AuthUserId";

        //    SqlCommand command = new SqlCommand(strCmd, conn);

        //    DBHelper.AddParam(command, "@AuthUserId", SqlDbType.VarChar, authUserId);

        //    String email = null;
        //    using (conn)
        //    {
        //        await conn.OpenAsync();
        //        using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //        {
        //            if (await reader.ReadAsync())
        //            {
        //                email = reader["Email"].ToString();
        //            }
        //        }
        //    }

        //    return email;
        //}


        // INSERT
        public async Task<int> Add(WebSysUser webSysUser)
        {
            String strCmd = $"INSERT INTO {table} (Roles, AuthUserId, Email, PhoneCountryId, Phone, Pin, PinFails, PinDateTime, CreateDateTime, UpdateDateTime, WebSysUserStatusId)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Roles, @AuthUserId, @Email, @PhoneCountryId, @Phone, @Pin, @PinFails, @PinDateTime, @CreateDateTime, @UpdateDateTime, @WebSysUserStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Roles", SqlDbType.VarChar, webSysUser.Roles);
            DBHelper.AddParam(command, "@AuthUserId", SqlDbType.VarChar, webSysUser.AuthUserId);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, webSysUser.Email);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, webSysUser.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, webSysUser.Phone);
            DBHelper.AddParam(command, "@Pin", SqlDbType.VarChar, webSysUser.Pin);
            DBHelper.AddParam(command, "@PinFails", SqlDbType.Int, webSysUser.PinFails);
            DBHelper.AddParam(command, "@PinDateTime", SqlDbType.DateTime2, webSysUser.PinDateTime);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@WebSysUserStatusId", SqlDbType.Int, webSysUser.WebSysUserStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(WebSysUser webSysUser)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Roles = @Roles, AuthUserId = @AuthUserId, Email = @Email, PhoneCountryId = @PhoneCountryId, Phone = @Phone," +
                            " UpdateDateTime = @UpdateDateTime, WebSysUserStatusId = @WebSysUserStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Roles", SqlDbType.VarChar, webSysUser.Roles);
            DBHelper.AddParam(command, "@AuthUserId", SqlDbType.VarChar, webSysUser.AuthUserId);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, webSysUser.Email);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, webSysUser.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, webSysUser.Phone);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@WebSysUserStatusId", SqlDbType.Int, webSysUser.WebSysUserStatusId);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, webSysUser.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateRoles(int id, String roles)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Roles = @Roles, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Roles", SqlDbType.VarChar, roles);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateMail(int id, String eMail)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Email = @Email, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, eMail);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdatePhone(PhoneRequest phoneRequest)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET PhoneCountryId = @PhoneCountryId, Phone = @Phone, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, phoneRequest.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, phoneRequest.Phone);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, phoneRequest.Id);

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

        // PIN
        public async Task<(String, int, DateTime?)> GetPin(int id)
        {
            String strCmd = $"SELECT Pin, PinFails, PinDateTime FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            (String pin, int fails, DateTime? dateTime) = (null, 0, null);
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        pin = reader["Pin"].ToString();
                        fails = Convert.ToInt32(reader["PinFails"]);
                        dateTime = reader["PinDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["PinDateTime"]);
                    }
                }
            }

            return (pin, fails, dateTime);
        }

        public async Task<bool> UpdatePin(WebSysPinRequest pinRequest)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET Pin = @Pin, PinFails = 0, PinDateTime = NULL, UpdateDateTime = @UpdateDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Pin", SqlDbType.VarChar, pinRequest.Pin);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, pinRequest.WebSysUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdatePinFails(int id, int pinFails, DateTime? pinDateTime = null)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET PinFails = @PinFails, PinDateTime = @PinDateTime" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PinFails", SqlDbType.Int, pinFails);
            DBHelper.AddParam(command, "@PinDateTime", SqlDbType.DateTime2, pinDateTime);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

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
