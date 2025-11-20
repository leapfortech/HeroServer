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
        readonly String table = "[D-BoardUser]";

        private static BoardUser GetBoardUser(SqlDataReader reader)
        {
            return new BoardUser(Convert.ToInt64(reader["Id"]),
                                 Convert.ToInt64(reader["WebSysUserId"]),
                                 Convert.ToInt64(reader["EntityId"]),
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
            String strCmd = $"SELECT {table}.Id AS Id, WebSysUserId, EntityId, {table}.CreateDateTime, {table}.UpdateDateTime, BoardUserStatusId," +
                             " Roles, AuthUserId, Email, PhoneCountryId, Phone, Pin, PinFails, PinDateTime, [D-WebSysUser].CreateDateTime AS WSUCreate, [D-WebSysUser].UpdateDateTime AS WSUUpdate, WebSysUserStatusId" +
                            $" FROM {table}" +
                             " INNER JOIN [D-WebSysUser] ON ([D-WebSysUser].Id = WebSysUserId)";

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

        public async Task<BoardUser> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

        public async Task<BoardUser> GetByIdStatus(long id, int boardUserStatusId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND BoardUserStatusId = @BoardUserStatusId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);
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

        public async Task<BoardUser> GetByWebSysUserId(long webSysUserId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysUserId);

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

        public async Task<long> GetWebSysUserId(long id)
        {
            String strCmd = $"SELECT WebSysUserId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            long webSysUserId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        webSysUserId = Convert.ToInt64(reader["WebSysUserId"]);
                    }
                }
            }

            return webSysUserId;
        }

        public async Task<long> GetIdByWebSysUserId(long webSysUserId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysUserId);

            long id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
                    }
                }
            }

            return id;
        }

        public async Task<long> GetIdByEmail(String eMail)
        {
            String strCmd = $"SELECT {table}.Id FROM {table}" +
                            $" INNER JOIN [D-WebSysUser] ON ([D-WebSysUser].Id = {table}.WebSysUserId)" +
                             " WHERE [D-WebSysUser].Email = @Email";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, eMail);

            long id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
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
        public async Task<long> Add(BoardUser boardUser)
        {
            String strCmd = $"INSERT INTO {table}(Id, WebSysUserId, EntityId, CreateDateTime, UpdateDateTime, BoardUserStatusId)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @WebSysUserId, @EntityId, @CreateDateTime, @UpdateDateTime, @BoardUserStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('B'));
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, boardUser.WebSysUserId);
            DBHelper.AddParam(command, "@EntityId", SqlDbType.BigInt, boardUser.EntityId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@BoardUserStatusId", SqlDbType.Int, boardUser.BoardUserStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(BoardUser boardUser)
        {
            String strCmd = $"UPDATE {table} SET WebSysUserId = @WebSysUserId, EntityId = @EntityId," +
                             " UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, boardUser.WebSysUserId);
            DBHelper.AddParam(command, "@EntityId", SqlDbType.BigInt, boardUser.EntityId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, boardUser.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, BoardUserStatusId = @BoardUserStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@BoardUserStatusId", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByWebSysUserId(long webSysUserId, int boardUserStatusId)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, BoardUserStatusId = @BoardUserStatusId" +
                            " WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@BoardUserStatusId", SqlDbType.Int, boardUserStatusId);
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysUserId);

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

        public async Task<bool> DeleteByWebSysUserId(long webSysUserid)
        {
            String strCmd = $"DELETE {table} WHERE WebSysUserid = @WebSysUserid";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserid", SqlDbType.BigInt, webSysUserid);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
