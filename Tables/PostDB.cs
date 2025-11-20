using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PostDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Post]";

        private static Post GetPost(SqlDataReader reader)
        {
            return new Post(Convert.ToInt64(reader["Id"]),
                            Convert.ToInt64(reader["AppUserId"]),
                            Convert.ToInt64(reader["PostTypeId"]),
                            Convert.ToInt64(reader["PostSubtypeId"]),
                            Convert.ToInt64(reader["OriginCountryId"]),
                            Convert.ToInt64(reader["OriginStateId"]),
                            reader["Title"].ToString(),
                            reader["Summary"].ToString(),
                            reader["Description"].ToString(),
                            Convert.ToInt32(reader["ImageCount"]),
                            Convert.ToInt32(reader["LikesCount"]),
                            Convert.ToDateTime(reader["PublicationDateTime"]),
                            reader["ApprovalDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ApprovalDateTime"]),
                            reader["ExpirationDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ExpirationDateTime"]),
                            Convert.ToDateTime(reader["CreateDateTime"]),
                            Convert.ToDateTime(reader["UpdateDateTime"]),
                            Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Post>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Post> posts = new List<Post>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Post post = GetPost(reader);
                         posts.Add(post);
                    }
                }
            }
            return posts;
        }

        public async Task<Post> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Post post = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         post = GetPost(reader);
                    }
                }
            }
            return post;
        }

        // INSERT
        public async Task<long> Add(Post post)
        {
            String strCmd = $"INSERT INTO {table}(Id, AppUserId, PostTypeId, PostSubtypeId, OriginCountryId, OriginStateId, Title, Summary, Description, ImageCount, LikesCount, PublicationDateTime, ApprovalDateTime, ExpirationDateTime, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @AppUserId, @PostTypeId, @PostSubtypeId, @OriginCountryId, @OriginStateId, @Title, @Summary, @Description, @ImageCount, @LikesCount, @PublicationDateTime, @ApprovalDateTime, @ExpirationDateTime, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, post.AppUserId);
            DBHelper.AddParam(command, "@PostTypeId", SqlDbType.BigInt, post.PostTypeId);
            DBHelper.AddParam(command, "@PostSubtypeId", SqlDbType.BigInt, post.PostSubtypeId);
            DBHelper.AddParam(command, "@OriginCountryId", SqlDbType.BigInt, post.OriginCountryId);
            DBHelper.AddParam(command, "@OriginStateId", SqlDbType.BigInt, post.OriginStateId);
            DBHelper.AddParam(command, "@Title", SqlDbType.VarChar, post.Title);
            DBHelper.AddParam(command, "@Summary", SqlDbType.VarChar, post.Summary);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, post.Description);
            DBHelper.AddParam(command, "@ImageCount", SqlDbType.Int, post.ImageCount);
            DBHelper.AddParam(command, "@LikesCount", SqlDbType.Int, post.LikesCount);
            DBHelper.AddParam(command, "@PublicationDateTime", SqlDbType.DateTime, post.PublicationDateTime);
            DBHelper.AddParam(command, "@ApprovalDateTime", SqlDbType.DateTime, post.ApprovalDateTime);
            DBHelper.AddParam(command, "@ExpirationDateTime", SqlDbType.DateTime, post.ExpirationDateTime);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, post.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Post post)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, PostTypeId = @PostTypeId, PostSubtypeId = @PostSubtypeId, OriginCountryId = @OriginCountryId, OriginStateId = @OriginStateId, Title = @Title, Summary = @Summary, Description = @Description, ImageCount = @ImageCount, LikesCount = @LikesCount, PublicationDateTime = @PublicationDateTime, ApprovalDateTime = @ApprovalDateTime, ExpirationDateTime = @ExpirationDateTime, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, post.AppUserId);
            DBHelper.AddParam(command, "@PostTypeId", SqlDbType.BigInt, post.PostTypeId);
            DBHelper.AddParam(command, "@PostSubtypeId", SqlDbType.BigInt, post.PostSubtypeId);
            DBHelper.AddParam(command, "@OriginCountryId", SqlDbType.BigInt, post.OriginCountryId);
            DBHelper.AddParam(command, "@OriginStateId", SqlDbType.BigInt, post.OriginStateId);
            DBHelper.AddParam(command, "@Title", SqlDbType.VarChar, post.Title);
            DBHelper.AddParam(command, "@Summary", SqlDbType.VarChar, post.Summary);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, post.Description);
            DBHelper.AddParam(command, "@ImageCount", SqlDbType.Int, post.ImageCount);
            DBHelper.AddParam(command, "@LikesCount", SqlDbType.Int, post.LikesCount);
            DBHelper.AddParam(command, "@PublicationDateTime", SqlDbType.DateTime, post.PublicationDateTime);
            DBHelper.AddParam(command, "@ApprovalDateTime", SqlDbType.DateTime, post.ApprovalDateTime);
            DBHelper.AddParam(command, "@ExpirationDateTime", SqlDbType.DateTime, post.ExpirationDateTime);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, post.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, post.Id);

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
