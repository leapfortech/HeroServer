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
                                 reader["Alias"].ToString(),
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

        public async Task<BoardUserFullAllRsp> GetFullAllByName(BoardUserAllByNameReq req)
        {
            req.Page = Math.Max(1, req.Page);
            req.PageSize = Math.Max(1, req.PageSize);

            int offset = (req.Page - 1) * req.PageSize;

            List<BoardUserFull> boardUserFulls = new List<BoardUserFull>();

            String strCmd = // Count
                              @"SELECT COUNT(BU.Id) AS TotalCount
                              FROM [D-BoardUser] BU
                              INNER JOIN [D-WebSysUser] WSU ON WSU.Id = BU.WebSysUserId
                              LEFT JOIN [J-IdentityBoardUser] JIBU ON JIBU.BoardUserId = BU.Id AND JIBU.Status = 1
                              LEFT JOIN [D-Identity] I ON I.Id = JIBU.IdentityId
                              WHERE (@Status = -1 OR BU.BoardUserStatusId = @Status)
                              AND (@Name IS NULL OR 
                                    (I.FirstName1 + ' ' + I.LastName1) LIKE '%' + @Name + '%' OR
                                    BU.Alias LIKE '%' + @Name + '%');" +

                              // BoardUser + WebSysUser
                              @"SELECT 
                                    BU.Id, BU.WebSysUserId, BU.Alias, BU.CreateDateTime,
                                    BU.UpdateDateTime, BU.BoardUserStatusId,
                                    WSU.Id AS WSUId, WSU.Roles, WSU.AuthUserId, WSU.Email, WSU.PhoneCountryId,
                                    WSU.Phone, WSU.Pin, WSU.PinFails, WSU.PinDateTime,
                                    WSU.CreateDateTime AS WSUCreate,
                                    WSU.UpdateDateTime AS WSUUpdate,
                                    WSU.WebSysUserStatusId
                              FROM [D-BoardUser] BU
                              INNER JOIN [D-WebSysUser] WSU ON WSU.Id = BU.WebSysUserId
                              LEFT JOIN [J-IdentityBoardUser] JIBU ON JIBU.BoardUserId = BU.Id AND JIBU.Status = 1
                              LEFT JOIN [D-Identity] I ON I.Id = JIBU.IdentityId
                              WHERE (@Status = -1 OR BU.BoardUserStatusId = @Status)
                              AND (@Name IS NULL OR 
                                    (I.FirstName1 + ' ' + I.LastName1) LIKE '%' + @Name + '%' OR
                                    BU.Alias LIKE '%' + @Name + '%')
                              ORDER BY BU.CreateDateTime DESC
                              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;" +

                              // Identity
                              @"SELECT 
                                    BU.Id AS BoardUserId,
                                    I.Id, I.FirstName1, I.FirstName2, I.LastName1, I.LastName2,
                                    I.GenderId, I.BirthDate, I.BirthCountryId, I.BirthStateId, I.BirthCityId,
                                    I.PhoneCountryId, I.Phone, I.Email,
                                    I.CreateDateTime, I.UpdateDateTime, I.Status
                              FROM [D-BoardUser] BU
                              INNER JOIN [J-IdentityBoardUser] JIBU ON JIBU.BoardUserId = BU.Id AND JIBU.Status = 1
                              INNER JOIN [D-Identity] I ON I.Id = JIBU.IdentityId
                              WHERE BU.Id IN (
                                    SELECT BU.Id
                                    FROM [D-BoardUser] BU
                                    LEFT JOIN [J-IdentityBoardUser] JIBU ON JIBU.BoardUserId = BU.Id AND JIBU.Status = 1
                                    LEFT JOIN [D-Identity] I ON I.Id = JIBU.IdentityId
                                    WHERE (@Status = -1 OR BU.BoardUserStatusId = @Status)
                                    AND (@Name IS NULL OR 
                                        (I.FirstName1 + ' ' + I.LastName1) LIKE '%' + @Name + '%' OR
                                        BU.Alias LIKE '%' + @Name + '%')
                                    ORDER BY BU.CreateDateTime DESC
                                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY);";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, req.Status);
            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, String.IsNullOrWhiteSpace(req.Name) ? DBNull.Value : req.Name);
            DBHelper.AddParam(command, "@Offset", SqlDbType.Int, offset);
            DBHelper.AddParam(command, "@PageSize", SqlDbType.Int, req.PageSize);

            int totalCount = 0;

            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    // 1. Count
                    if (await reader.ReadAsync())
                        totalCount = Convert.ToInt32(reader["TotalCount"]);

                    int totalPages = (int)Math.Ceiling((double)totalCount / req.PageSize);

                    // 2. BoardUser + WebSysUser
                    await reader.NextResultAsync();
                    while (await reader.ReadAsync())
                    {
                        BoardUser boardUser = GetBoardUser(reader);

                        WebSysUser webSysUser = WebSysUserDB.GetWebSysUser(reader);
                        webSysUser.Id = Convert.ToInt64(reader["WSUId"]);
                        webSysUser.CreateDateTime = Convert.ToDateTime(reader["WSUCreate"]);
                        webSysUser.UpdateDateTime = Convert.ToDateTime(reader["WSUUpdate"]);

                        boardUserFulls.Add(new BoardUserFull(boardUser, webSysUser, null));
                    }

                    // 3. Identity
                    await reader.NextResultAsync();
                    while (await reader.ReadAsync())
                    {
                        long boardUserId = Convert.ToInt64(reader["BoardUserId"]);
                        Identity identity = IdentityDB.GetIdentity(reader);

                        for (int i = 0; i < boardUserFulls.Count; i++)
                        {
                            if (boardUserFulls[i].BoardUser.Id == boardUserId)
                            {
                                boardUserFulls[i].Identity = identity;
                                break;
                            }
                        }
                    }

                    return new BoardUserFullAllRsp(req.Page, totalPages, boardUserFulls);
                }
            }
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
            String strCmd = $"INSERT INTO {table}(Id, WebSysUserId, Alias, CreateDateTime, UpdateDateTime, BoardUserStatusId)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @WebSysUserId, @Alias, @CreateDateTime, @UpdateDateTime, @BoardUserStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('B'));
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, boardUser.WebSysUserId);
            DBHelper.AddParam(command, "@Alias", SqlDbType.VarChar, boardUser.Alias);
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
            String strCmd = $"UPDATE {table} SET WebSysUserId = @WebSysUserId, Alias = @Alias," +
                             " UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, boardUser.WebSysUserId);
            DBHelper.AddParam(command, "@Alias", SqlDbType.VarChar, boardUser.Alias);
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
