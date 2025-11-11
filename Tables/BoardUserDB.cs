using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class BoardUserDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-BoardUser]";

        private static BoardUser GetBoardUser(SqlDataReader reader)
        {
            return new BoardUser(Convert.ToInt32(reader["Id"]),
                                 Convert.ToInt32(reader["WebSysUserId"]),
                                 reader["FirstName1"].ToString(),
                                 reader["FirstName2"].ToString(),
                                 reader["LastName1"].ToString(),
                                 reader["LastName2"].ToString(),
                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                 Convert.ToDateTime(reader["UpdateDateTime"]),
                                 Convert.ToInt32(reader["BoardUserStatusId"]));
        }

        // GET
        public async Task<IEnumerable<BoardUser>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<BoardUser> boardUsers = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         BoardUser boardUser = GetBoardUser(reader);
                         boardUsers.Add(boardUser);
                    }
                }
            }
            return boardUsers;
        }

        public async Task<IEnumerable<BoardUserFull>> GetFulls()
        {
            String strCmd = $"SELECT {table}.Id AS Id, WebSysUserId, FirstName1, FirstName2, LastName1, LastName2, {table}.CreateDateTime, {table}.UpdateDateTime, BoardUserStatusId," +
                             " Roles, AuthUserId, Email, PhoneCountryId, Phone, Pin, PinFails, PinDateTime, [DD-WebSysUser].CreateDateTime AS WSUCreate, [DD-WebSysUser].UpdateDateTime AS WSUUpdate, WebSysUserStatusId" +
                            $" FROM {table}" +
                             " INNER JOIN [DD-WebSysUser] ON ([DD-WebSysUser].Id = WebSysUserId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<BoardUserFull> boardUserFulls = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        BoardUser boardUser = GetBoardUser(reader);
                        WebSysUser webSysUser = WebSysUserDB.GetWebSysUser(reader);
                        webSysUser.Id = boardUser.WebSysUserId;
                        webSysUser.CreateDateTime = Convert.ToDateTime(reader["WSUCreate"]);
                        webSysUser.UpdateDateTime = Convert.ToDateTime(reader["WSUUpdate"]);
                        boardUserFulls.Add(new BoardUserFull(boardUser, webSysUser));
                    }
                }
            }
            return boardUserFulls;
        }

        public async Task<BoardUser> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            BoardUser boardUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        boardUser = GetBoardUser(reader);
                    }
                }
            }
            return boardUser;
        }

        public async Task<BoardUser> GetByIdStatus(int id, int boardUserStatusId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND BoardUserStatusId = @BoardUserStatusId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);
            DBHelper.AddParam(command, "@BoardUserStatusId", SqlDbType.Int, boardUserStatusId);

            BoardUser boardUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        boardUser = GetBoardUser(reader);
                    }
                }
            }
            return boardUser;
        }

        public async Task<BoardUser> GetByWebSysUserId(int webSysUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);

            BoardUser boardUser = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        boardUser = GetBoardUser(reader);
                    }
                }
            }
            return boardUser;
        }

        public async Task<int> GetWebSysUserId(int id)
        {
            String strCmd = $"SELECT WebSysUserId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            int webSysUserId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        webSysUserId = Convert.ToInt32(reader["WebSysUserId"]);
                    }
                }
            }

            return webSysUserId;
        }

        public async Task<int> GetIdByWebSysUserId(int webSysUserId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);

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

        public async Task<int> GetIdByEmail(String eMail)
        {
            String strCmd = $"SELECT {table}.Id FROM {table}" +
                            $" INNER JOIN [DD-WebSysUser] ON ([DD-WebSysUser].Id = {table}.WebSysUserId)" +
                             " WHERE [DD-WebSysUser].Email = @Email";

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

        public async Task<int> GetCountAll()
        {
            String strCmd = $"SELECT COUNT(Id) Count FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

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

        public async Task<int> GetCountByStatus(int appUserStatusId)
        {
            String strCmd = $"SELECT COUNT(BoardUserStatusId) Count FROM {table}";
            if (appUserStatusId >= 0)
                strCmd += " WHERE BoardUserStatusId = @BoardUserStatusId";
            else
                strCmd += " WHERE BoardUserStatusId >= @BoardUserStatusId";

            SqlCommand command = new SqlCommand(strCmd, conn);

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
        public async Task<int> Add(BoardUser boardUser)
        {
            String strCmd = $"INSERT INTO {table}(WebSysUserId, FirstName1, FirstName2, LastName1, LastName2, CreateDateTime, UpdateDateTime, BoardUserStatusId)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@WebSysUserId, @FirstName1, @FirstName2, @LastName1, @LastName2, @CreateDateTime, @UpdateDateTime, @BoardUserStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, boardUser.WebSysUserId);
            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, boardUser.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, boardUser.FirstName2);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, boardUser.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, boardUser.LastName2);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@BoardUserStatusId", SqlDbType.Int, boardUser.BoardUserStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(BoardUser boardUser)
        {
            String strCmd = $"UPDATE {table} SET WebSysUserId = @WebSysUserId, FirstName1 = @FirstName1, FirstName2 = @FirstName2, LastName1 = @LastName1, LastName2 = @LastName2," +
                             " UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, boardUser.WebSysUserId);
            DBHelper.AddParam(command, "@FirstName1", SqlDbType.VarChar, boardUser.FirstName1);
            DBHelper.AddParam(command, "@FirstName2", SqlDbType.VarChar, boardUser.FirstName2);
            DBHelper.AddParam(command, "@LastName1", SqlDbType.VarChar, boardUser.LastName1);
            DBHelper.AddParam(command, "@LastName2", SqlDbType.VarChar, boardUser.LastName2);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, boardUser.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, BoardUserStatusId = @BoardUserStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@BoardUserStatusId", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByWebSysUserId(int webSysUserId, int boardUserStatusId)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, BoardUserStatusId = @BoardUserStatusId" +
                            " WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@BoardUserStatusId", SqlDbType.Int, boardUserStatusId);
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() > 0;
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

        public async Task<bool> DeleteByWebSysUserId(int webSysUserid)
        {
            String strCmd = $"DELETE {table} WHERE WebSysUserid = @WebSysUserid";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserid", SqlDbType.Int, webSysUserid);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
