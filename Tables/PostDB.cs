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
                            Convert.ToInt64(reader["CountryId"]),
                            Convert.ToInt64(reader["StateId"]),
                            reader["Title"].ToString(),
                            reader["Summary"].ToString(),
                            reader["Description"].ToString(),
                            Convert.ToInt32(reader["ImageCount"]),
                            Convert.ToInt32(reader["LikeCount"]),
                            Convert.ToDateTime(reader["PublicationDateTime"]),
                            reader["ApprovalDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ApprovalDateTime"]),
                            reader["ExpirationDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ExpirationDateTime"]),
                            Convert.ToDateTime(reader["CreateDateTime"]),
                            Convert.ToDateTime(reader["UpdateDateTime"]),
                            Convert.ToInt32(reader["Status"]));
        }

        public static PostFull GetPostFull(SqlDataReader reader)
        {
            return new PostFull(Convert.ToInt64(reader["PostId"]),
                                Convert.ToInt64(reader["AppUserId"]),
                                reader["AppUserAlias"].ToString(),
                                Convert.ToInt64(reader["PostTypeId"]),
                                Convert.ToInt64(reader["PostCountryId"]),
                                Convert.ToInt64(reader["PostStateId"]),
                                reader["Title"].ToString(),
                                null,   //TitleImage
                                reader["Summary"].ToString(),
                                reader["Description"].ToString(),
                                Convert.ToInt32(reader["ImageCount"]),
                                Convert.ToInt32(reader["LikeCount"]),
                                Convert.ToDateTime(reader["PublicationDateTime"]),
                                Convert.ToInt32(reader["PostStatus"]),
                                null,   //ContactFull
                                null,   //LinkFulls
                                null);  //CommentFulls
        }


        // GET
        public async Task<List<Post>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Post> posts = [];
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

        public async Task<long> GetPostTypeId(long id)
        {
            String strCmd = $"SELECT PostTypeId FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            long postTypeId = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        postTypeId = Convert.ToInt64(reader["PostTypeId"]);
                    }
                }
            }
            return postTypeId;
        }

        public async Task<int> GetImageCount(long id)
        {
            String strCmd = $"SELECT ImageCount FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            int imageCount = 0;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        imageCount = Convert.ToInt32(reader["ImageCount"]);
                    }
                }
            }
            return imageCount;
        }

        public async Task<PostFeedResponse> GetPostFeed(PostFeedRequest request)
        {
            PostFeedResponse response = new PostFeedResponse(request.PageSize);

            FeedCursor cursor = FeedCursor.DecodeCursor(request.Cursor);

            // FILTERS
            List<String> where = [];

            if (request.AppUserId != -1)
                where.Add("Post.AppUserId = @AppUserId");

            if (request.PostTypeId != -1)
                where.Add("Post.PostTypeId = @PostTypeId");

            if (request.CountryId != -1)
                where.Add("Post.CountryId = @CountryId");

            if (request.StateId != -1)
                where.Add("Post.StateId = @StateId");

            if (request.Status != -1)
                where.Add("Post.Status = @Status");

            String whereCountClause = where.Count > 0 ? " WHERE " + String.Join(" AND ", where) : "";

            // CURSOR
            List<String> whereFeed = [.. where];

            if (cursor != null)
            {
                if (request.Direction == 1) // REFRESH
                {
                    whereFeed.Add("(Post.PublicationDateTime > @CursorDate" +
                                  " OR (Post.PublicationDateTime = @CursorDate AND Post.Id > @CursorPostId))"
                    );
                }
                else if (request.Direction == 2) // OLDER
                {
                    whereFeed.Add("(Post.PublicationDateTime < @CursorDate" +
                                  " OR (Post.PublicationDateTime = @CursorDate AND Post.Id < @CursorPostId))"
                    );
                }
            }

            String whereFeedClause = whereFeed.Count > 0 ? " WHERE " + String.Join(" AND ", whereFeed) : "";

            // QUERY FEED
            String strCmd = "SELECT TOP (@PageSize)" +
                            " Post.Id AS PostId," +
                            " Post.AppUserId," +
                            " AppUser.Alias AS AppUserAlias," +
                            " Post.PostTypeId," +
                            " Post.CountryId AS PostCountryId," +
                            " Post.StateId AS PostStateId," +
                            " Post.Title," +
                            " Post.Summary," +
                            " Post.Description," +
                            " Post.ImageCount," +
                            " Post.LikeCount," +
                            " Post.PublicationDateTime," +
                            " Post.Status AS PostStatus" +
                            " FROM [D-Post] AS Post" +
                            " INNER JOIN [D-AppUser] AS AppUser ON Post.AppUserId = AppUser.Id" +
                            whereFeedClause +
                            " ORDER BY Post.PublicationDateTime DESC, Post.Id DESC;";

            // QUERY COUNT
            strCmd += "SELECT COUNT(1) FROM [D-Post] AS Post " +
                      " INNER JOIN [D-AppUser] AS AppUser ON Post.AppUserId = AppUser.Id " +
                      whereCountClause + ";";


            using (SqlCommand command = new SqlCommand(strCmd, conn))
            {
                DBHelper.AddParam(command, "@PageSize", SqlDbType.Int, request.PageSize);
                
                if(request.AppUserId != -1)
                    DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, request.AppUserId);

                if (request.PostTypeId != -1)
                    DBHelper.AddParam(command, "@PostTypeId", SqlDbType.BigInt, request.PostTypeId);

                if (request.CountryId != -1)
                    DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, request.CountryId);

                if (request.StateId != -1)
                    DBHelper.AddParam(command, "@StateId", SqlDbType.BigInt, request.StateId);

                if (request.Status != -1)
                    DBHelper.AddParam(command, "@Status", SqlDbType.Int, request.Status);


                if (cursor != null)
                {
                    DBHelper.AddParam(command, "@CursorDate", SqlDbType.DateTime2, cursor.PublicationDateTime);
                    DBHelper.AddParam(command, "@CursorPostId", SqlDbType.BigInt, cursor.PostId);
                }

                using (conn)
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            response.PostFulls.Add(PostDB.GetPostFull(reader));

                        await reader.NextResultAsync();
                        if (await reader.ReadAsync())
                            response.Total = reader.GetInt32(0);
                    }
                }
            }

            if (response.PostFulls.Count > 0)
            {
                PostFull first = response.PostFulls[0];
                PostFull last = response.PostFulls[^1];

                response.PrevCursor = FeedCursor.EncodeCursor(first);
                response.NextCursor = FeedCursor.EncodeCursor(last);
            }

            return response;
        }

        // INSERT
        public async Task<long> Add(Post post)
        {
            String strCmd = $"INSERT INTO {table}(Id, AppUserId, PostTypeId, CountryId, StateId, Title, Summary, Description, ImageCount, LikeCount, PublicationDateTime, ApprovalDateTime, ExpirationDateTime, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @AppUserId, @PostTypeId, @CountryId, @StateId, @Title, @Summary, @Description, @ImageCount, @LikeCount, @PublicationDateTime, @ApprovalDateTime, @ExpirationDateTime, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('P'));
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, post.AppUserId);
            DBHelper.AddParam(command, "@PostTypeId", SqlDbType.BigInt, post.PostTypeId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, post.CountryId);
            DBHelper.AddParam(command, "@StateId", SqlDbType.BigInt, post.StateId);
            DBHelper.AddParam(command, "@Title", SqlDbType.VarChar, post.Title);
            DBHelper.AddParam(command, "@Summary", SqlDbType.VarChar, post.Summary);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, post.Description);
            DBHelper.AddParam(command, "@ImageCount", SqlDbType.Int, post.ImageCount);
            DBHelper.AddParam(command, "@LikeCount", SqlDbType.Int, post.LikeCount);
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
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, PostTypeId = @PostTypeId, CountryId = @CountryId, StateId = @StateId, Title = @Title, Summary = @Summary, Description = @Description, ImageCount = @ImageCount, LikeCount = @LikeCount, PublicationDateTime = @PublicationDateTime, ApprovalDateTime = @ApprovalDateTime, ExpirationDateTime = @ExpirationDateTime, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, post.AppUserId);
            DBHelper.AddParam(command, "@PostTypeId", SqlDbType.BigInt, post.PostTypeId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, post.CountryId);
            DBHelper.AddParam(command, "@StateId", SqlDbType.BigInt, post.StateId);
            DBHelper.AddParam(command, "@Title", SqlDbType.VarChar, post.Title);
            DBHelper.AddParam(command, "@Summary", SqlDbType.VarChar, post.Summary);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, post.Description);
            DBHelper.AddParam(command, "@ImageCount", SqlDbType.Int, post.ImageCount);
            DBHelper.AddParam(command, "@LikeCount", SqlDbType.Int, post.LikeCount);
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

        public async Task<bool> UpdateImageCount(long id, int imageCount)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET ImageCount = @ImageCount" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ImageCount", SqlDbType.Int, imageCount);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> IncrementLikeCount(long id)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET LikeCount = LikeCount + 1" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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
