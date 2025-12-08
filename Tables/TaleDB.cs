using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class TaleDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Tale]";

        private static Tale GetTale(SqlDataReader reader)
        {
            return new Tale(Convert.ToInt64(reader["Id"]),
                            Convert.ToInt64(reader["PostId"]),
                            Convert.ToDateTime(reader["CreateDateTime"]),
                            Convert.ToDateTime(reader["UpdateDateTime"]),
                            Convert.ToInt32(reader["Status"]));
        }

        public static TaleFull GetTaleFull(SqlDataReader reader)
        {
            return new TaleFull(Convert.ToInt64(reader["Id"]),

                                Convert.ToInt64(reader["PostId"]),
                                Convert.ToInt64(reader["AppUserId"]),
                                reader["AppUserAlias"].ToString(),
                                Convert.ToInt64(reader["PostTypeId"]),
                                Convert.ToInt64(reader["PostSubtypeId"]),
                                Convert.ToInt64(reader["PostOriginCountryId"]),
                                Convert.ToInt64(reader["PostOriginStateId"]),
                                reader["Title"].ToString(),
                                reader["Summary"].ToString(),
                                reader["Description"].ToString(),
                                Convert.ToInt32(reader["ImageCount"]),
                                Convert.ToInt32(reader["LikeCount"]),
                                Convert.ToDateTime(reader["PublicationDateTime"]),
                                Convert.ToInt32(reader["PostStatus"]),

                                Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Tale>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Tale> tales = new List<Tale>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Tale tale = GetTale(reader);
                         tales.Add(tale);
                    }
                }
            }
            return tales;
        }

        public async Task<Tale> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Tale tale = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         tale = GetTale(reader);
                    }
                }
            }
            return tale;
        }

        // GET FULL
        public async Task<TaleFull> GetFullById(long id)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias," +
                             " Post.PostTypeId, Post.PostSubtypeId," +
                             " Post.PostOriginCountryId, Post.PostOriginStateId," +
                             " Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.Id = @Id;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            TaleFull taleFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    taleFull = GetTaleFull(reader);
                }
            }

            return taleFull;
        }

        public async Task<TaleFull> GetFullByPostId(long postId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                            " Post.AppUserId, AppUser.Alias AS AppUserAlias," +
                            " Post.PostTypeId, Post.PostSubtypeId," +
                            " Post.PostOriginCountryId, Post.PostOriginStateId, Post.Title, Post.Summary, Post.Description," +
                            " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.PostId = @PostId;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);

            TaleFull taleFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    taleFull = GetTaleFull(reader);
                }
            }

            return taleFull;
        }

        public async Task<List<TaleFull>> GetFullsByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId, Post.PostSubtypeId," +
                             " Post.PostOriginCountryId, Post.PostOriginStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)";


            if (status != -1)
                strCmd += $" WHERE {table}.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<TaleFull> taleFulls = [];
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        TaleFull taleFull = GetTaleFull(reader);
                        taleFulls.Add(taleFull);
                    }
                }
            }

            return taleFulls;
        }

        // INSERT
        public async Task<long> Add(Tale tale)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, tale.PostId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, tale.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Tale tale)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, tale.PostId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, tale.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, tale.Id);

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
