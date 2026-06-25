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

        static int taleExpirationTime, recipeExpirationTime, treatmentExpirationTime, radioExpirationTime, productExpirationTime, happeningExpirationTime, newsExpirationTime;

        public static void InitParams(int taleExpTime, int recipeExpTime, int treatmentExpTime, int radioExpTime, int productExpTime, int happeningExpTime, int newsExpTime)
        {
            taleExpirationTime = taleExpTime;
            recipeExpirationTime = recipeExpTime;
            treatmentExpirationTime = treatmentExpTime;
            radioExpirationTime = radioExpTime;
            productExpirationTime = productExpTime;
            happeningExpirationTime = happeningExpTime;
            newsExpirationTime = newsExpTime;
        }

        public static Post GetPost(SqlDataReader reader)
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
                                Convert.ToInt32(reader["Favorite"]),
                                Convert.ToInt32(reader["Like"]),
                                Convert.ToInt32(reader["LikeCount"]),
                                Convert.ToInt64(reader["ReactionPhraseId"]),
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
                command.AddParam("@Status", SqlDbType.Int, status);

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

            command.AddParam("@Id", SqlDbType.BigInt, id);

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

            command.AddParam("@Id", SqlDbType.BigInt, id);

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

            command.AddParam("@Id", SqlDbType.BigInt, id);

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
            PostFeedResponse response = new PostFeedResponse(request.Chunk, request.Direction, request.Count);

            // FILTERS
            List<String> where = [];

            if (request.PostTypeId != -1)
                where.Add("Post.PostTypeId = @PostTypeId");

            if (request.AppUserId != -1)
                where.Add("Post.AppUserId = @AppUserId");

            if (request.CountryId != -1)
                where.Add("Post.CountryId = @CountryId");

            if (request.StateId != -1)
                where.Add("Post.StateId = @StateId");

            if (request.Status != -1)
                where.Add("Post.Status = @Status");

            // EXPIRATION
            if (taleExpirationTime > 0)
                where.Add($"(Post.PostTypeId != {(long)PostType.Tale} OR Post.PublicationDateTime >= DATEADD(DAY, -{taleExpirationTime}, GETDATE()))");

            if (recipeExpirationTime > 0)
                where.Add($"(Post.PostTypeId != {(long)PostType.Recipe} OR Post.PublicationDateTime >= DATEADD(DAY, -{recipeExpirationTime}, GETDATE()))");

            if (treatmentExpirationTime > 0)
                where.Add($"(Post.PostTypeId != {(long)PostType.Treatment} OR Post.PublicationDateTime >= DATEADD(DAY, -{treatmentExpirationTime}, GETDATE()))");

            if (radioExpirationTime > 0)
                where.Add($"(Post.PostTypeId != {(long)PostType.Radio} OR Post.PublicationDateTime >= DATEADD(DAY, -{radioExpirationTime}, GETDATE()))");

            if (productExpirationTime > 0)
                where.Add($"(Post.PostTypeId != {(long)PostType.Product} OR Post.PublicationDateTime >= DATEADD(DAY, -{productExpirationTime}, GETDATE()))");

            if (happeningExpirationTime > 0)
                where.Add($"(Post.PostTypeId != {(long)PostType.Happening} OR Post.PublicationDateTime >= DATEADD(DAY, -{happeningExpirationTime}, GETDATE()))");

            if (newsExpirationTime > 0)
                where.Add($"(Post.PostTypeId != {(long)PostType.News} OR Post.PublicationDateTime >= DATEADD(DAY, -{newsExpirationTime}, GETDATE()))");

            String whereCount = where.Count > 0 ? " WHERE " + String.Join(" AND ", where) : "";

            // DATE
            if (request.Direction == 1)
                where.Add("Post.PublicationDateTime > @StartDate");
            else
                where.Add("Post.PublicationDateTime < @StartDate");

            String whereFeed = where.Count > 0 ? " WHERE " + String.Join(" AND ", where) : "";

            // QUERY FEED
            String strCmd;

            if (request.Direction == 1)
                strCmd = "WITH Posts AS" +
                         " (SELECT ROW_NUMBER() OVER (ORDER BY Temp.PublicationDateTime DESC) AS RowNumber, * FROM" +
                         " (SELECT TOP(@Count2)";
            else
                strCmd = "SELECT TOP(@Count)";
            strCmd +=   " Post.Id AS PostId," +
                        " Post.AppUserId," +
                        " DAppUser.Alias AS AppUserAlias," +
                        " Post.PostTypeId," +
                        " Post.CountryId AS PostCountryId," +
                        " Post.StateId AS PostStateId," +
                        " Post.Title," +
                        " Post.Summary," +
                        " Post.Description," +
                        " Post.ImageCount," +
                        " CASE WHEN JFavorite.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                        " ISNULL(DLike.[Rank], -1) AS [Like]," +
                        " ISNULL(DReaction.[ReactionPhraseId], -1) AS [ReactionPhraseId]," +
                        " Post.LikeCount," +
                        " Post.PublicationDateTime," +
                        " Post.Status AS PostStatus" +
                        " FROM [D-Post] AS Post" +
                        " INNER JOIN [D-AppUser] AS DAppUser ON Post.AppUserId = DAppUser.Id" +
                        " LEFT JOIN [J-Favorite] AS JFavorite ON JFavorite.PostId = Post.Id AND JFavorite.AppUserId = @LikeAppUserId" +
                        " LEFT JOIN [D-Like] AS DLike ON DLike.PostId = Post.Id AND DLike.AppUserId = @LikeAppUserId" +
                        " LEFT JOIN [D-Reaction] AS DReaction ON DReaction.PostId = Post.Id AND DReaction.AppUserId = @LikeAppUserId" +
                        whereFeed +
                        " ORDER BY Post.PublicationDateTime";
            if (request.Direction == 1)
                strCmd += ") AS Temp)," +
                          " PostCount AS (SELECT COUNT(1) AS Total FROM Posts)" +
                          " SELECT * FROM Posts, PostCount" +
                          " WHERE RowNumber <= Total - @Count" +
                          " ORDER BY PublicationDateTime";
            strCmd += " DESC;";

            // QUERY COUNT
            strCmd += "SELECT COUNT(1) AS Total FROM [D-Post] AS Post" + whereCount + ";";
            strCmd += "SELECT TOP(1) Post.Id AS FirstPostId, Post.PublicationDateTime AS FirstDateTime FROM [D-Post] AS Post" + whereCount + ";";
            strCmd += "SELECT TOP(1) Post.Id AS LastPostId, Post.PublicationDateTime AS LastDateTime FROM [D-Post] AS Post" + whereCount + " ORDER BY Post.PublicationDateTime DESC;";

            using (SqlCommand command = new SqlCommand(strCmd, conn))
            {
                if (request.Direction == 1)
                    command.AddParam("@Count2", SqlDbType.Int, request.Count * 2);
                command.AddParam("@Count", SqlDbType.Int, request.Count);
                command.AddParam("@LikeAppUserId", SqlDbType.BigInt, request.LikeAppUserId);

                if (request.PostTypeId != -1)
                    command.AddParam("@PostTypeId", SqlDbType.BigInt, request.PostTypeId);

                if (request.AppUserId != -1)
                    command.AddParam("@AppUserId", SqlDbType.BigInt, request.AppUserId);

                if (request.CountryId != -1)
                    command.AddParam("@CountryId", SqlDbType.BigInt, request.CountryId);

                if (request.StateId != -1)
                    command.AddParam("@StateId", SqlDbType.BigInt, request.StateId);

                if (request.Status != -1)
                    command.AddParam("@Status", SqlDbType.Int, request.Status);


                command.AddParam("@StartDate", SqlDbType.DateTime2, request.StartDateTime);

                using (conn)
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            response.PostFulls.Add(PostDB.GetPostFull(reader));

                        await reader.NextResultAsync();
                        if (await reader.ReadAsync())
                            response.Total = Convert.ToInt32(reader["Total"]);

                        await reader.NextResultAsync();
                        if (await reader.ReadAsync())
                        {
                            response.FirstPostId = Convert.ToInt64(reader["FirstPostId"]);
                            response.FirstDateTime = Convert.ToDateTime(reader["FirstDateTime"]);
                        }

                        await reader.NextResultAsync();
                        if (await reader.ReadAsync())
                        {
                            response.LastPostId = Convert.ToInt64(reader["LastPostId"]);
                            response.LastDateTime = Convert.ToDateTime(reader["LastDateTime"]);
                        }
                    }
                }
            }

            return response;
        }

        public async Task<CommentFeedResponse> GetCommentFeed(CommentFeedRequest request)
        {
            CommentFeedResponse response = new CommentFeedResponse(request.Chunk, request.Direction, request.Count);

            // FILTERS
            List<String> where = [];

            // PostId
            where.Add("Comment.PostId = @PostId");

            if (request.AppUserId != -1)
                where.Add("Comment.AppUserId = @AppUserId");

            if (request.Status != -1)
                where.Add("Comment.Status = @Status");

            String whereCount = where.Count > 0 ? " WHERE " + String.Join(" AND ", where) : "";

            // DATE
            if (request.Direction == 1)
                where.Add("Comment.CreateDateTime > @StartDate");
            else
                where.Add("Comment.CreateDateTime < @StartDate");

            String whereFeed = where.Count > 0 ? " WHERE " + String.Join(" AND ", where) : "";

            // QUERY FEED
            String strCmd;

            if (request.Direction == 1)
                strCmd = "WITH Comments AS" +
                         " (SELECT ROW_NUMBER() OVER (ORDER BY Temp.CreateDateTime DESC) AS RowNumber, * FROM" +
                         " (SELECT TOP(@Count2)";
            else
                strCmd = "SELECT TOP(@Count)";

            strCmd += " Comment.Id," +
                      " Comment.PostId," +
                      " Comment.AppUserId," +
                      " DAppUser.Alias AS AppUserAlias," +
                      " Comment.Message," +
                      " Comment.CreateDateTime," +
                      " Comment.UpdateDateTime," +
                      " Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS DAppUser ON Comment.AppUserId = DAppUser.Id" +
                      whereFeed +
                      " ORDER BY Comment.CreateDateTime";

            if (request.Direction == 1)
                strCmd += ") AS Temp)," +
                          " CommentCount AS (SELECT COUNT(1) AS Total FROM Comments)" +
                          " SELECT * FROM Comments, CommentCount" +
                          " WHERE RowNumber <= Total - @Count" +
                          " ORDER BY CreateDateTime";

            strCmd += " DESC;";

            // QUERY COUNT
            strCmd += "SELECT COUNT(1) AS Total FROM [D-Comment] AS Comment" + whereCount + ";";
            strCmd += "SELECT TOP(1) Comment.Id AS FirstCommentId, Comment.CreateDateTime AS FirstDateTime FROM [D-Comment] AS Comment" + whereCount + " ORDER BY Comment.CreateDateTime;";
            strCmd += "SELECT TOP(1) Comment.Id AS LastCommentId, Comment.CreateDateTime AS LastDateTime FROM [D-Comment] AS Comment" + whereCount + " ORDER BY Comment.CreateDateTime DESC;";

            using (SqlCommand command = new SqlCommand(strCmd, conn))
            {
                if (request.Direction == 1)
                    command.AddParam("@Count2", SqlDbType.Int, request.Count * 2);

                command.AddParam("@Count", SqlDbType.Int, request.Count);
                command.AddParam("@PostId", SqlDbType.BigInt, request.PostId);

                if (request.AppUserId != -1)
                    command.AddParam("@AppUserId", SqlDbType.BigInt, request.AppUserId);

                if (request.Status != -1)
                    command.AddParam("@Status", SqlDbType.Int, request.Status);

                command.AddParam("@StartDate", SqlDbType.DateTime2, request.StartDateTime);

                using (conn)
                {
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            response.CommentFulls.Add(CommentDB.GetCommentFull(reader));

                        await reader.NextResultAsync();
                        if (await reader.ReadAsync())
                            response.Total = Convert.ToInt32(reader["Total"]);

                        await reader.NextResultAsync();
                        if (await reader.ReadAsync())
                        {
                            response.FirstCommentId = Convert.ToInt64(reader["FirstCommentId"]);
                            response.FirstDateTime = Convert.ToDateTime(reader["FirstDateTime"]);
                        }

                        await reader.NextResultAsync();
                        if (await reader.ReadAsync())
                        {
                            response.LastCommentId = Convert.ToInt64(reader["LastCommentId"]);
                            response.LastDateTime = Convert.ToDateTime(reader["LastDateTime"]);
                        }
                    }
                }
            }

            return response;
        }

        public async Task<PostFullsPagedResponse> GetFullsPagedByType(PostTypePagedRequest request)
        {
            request.Page = Math.Max(1, request.Page);
            request.PageSize = Math.Max(1, request.PageSize);

            int offset = (request.Page - 1) * request.PageSize;

            List<PostFull> postFulls = [];

            String strCmd = // Count
                            @"SELECT COUNT(Post.Id) AS TotalCount
                                FROM [D-Post] AS Post
                                WHERE (@PostTypeId = -1 OR Post.PostTypeId = @PostTypeId)
                                AND (@Status = -1 OR Post.Status = @Status);" +

                            // Posts
                            @"SELECT
                                Post.Id AS PostId,
                                Post.AppUserId,
                                AppUser.Alias AS AppUserAlias,
                                Post.PostTypeId,
                                Post.CountryId AS PostCountryId,
                                Post.StateId AS PostStateId,
                                Post.Title,
                                Post.Summary,
                                Post.Description,
                                Post.ImageCount,
                                0 AS Favorite,
                                -1 AS [Like],
                                -1 AS [ReactionPhraseId],
                                Post.LikeCount,
                                Post.PublicationDateTime,
                                Post.Status AS PostStatus
                                FROM [D-Post] AS Post
                                INNER JOIN [D-AppUser] AS AppUser 
                                    ON Post.AppUserId = AppUser.Id
                                WHERE (@PostTypeId = -1 OR Post.PostTypeId = @PostTypeId)
                                AND (@Status = -1 OR Post.Status = @Status)
                                ORDER BY Post.PublicationDateTime ASC
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostTypeId", SqlDbType.BigInt, request.PostTypeId);
            command.AddParam("@Status", SqlDbType.Int, request.Status);
            command.AddParam("@Offset", SqlDbType.Int, offset);
            command.AddParam("@PageSize", SqlDbType.Int, request.PageSize);

            int totalCount = 0;

            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    // 1. Count
                    if (await reader.ReadAsync())
                        totalCount = Convert.ToInt32(reader["TotalCount"]);

                    int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                    // 2. Posts
                    await reader.NextResultAsync();
                    while (await reader.ReadAsync())
                    {
                        PostFull postFull = GetPostFull(reader);
                        postFulls.Add(postFull);
                    }

                    return new PostFullsPagedResponse(request.Page, totalPages, postFulls);
                }
            }
        }

        // INSERT
        public async Task<long> Add(Post post)
        {
            String strCmd = $"INSERT INTO {table}(Id, AppUserId, PostTypeId, CountryId, StateId, Title, Summary, Description, ImageCount, LikeCount, PublicationDateTime, ApprovalDateTime, ExpirationDateTime, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @AppUserId, @PostTypeId, @CountryId, @StateId, @Title, @Summary, @Description, @ImageCount, @LikeCount, @PublicationDateTime, @ApprovalDateTime, @ExpirationDateTime, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('P'));
            command.AddParam("@AppUserId", SqlDbType.BigInt, post.AppUserId);
            command.AddParam("@PostTypeId", SqlDbType.BigInt, post.PostTypeId);
            command.AddParam("@CountryId", SqlDbType.BigInt, post.CountryId);
            command.AddParam("@StateId", SqlDbType.BigInt, post.StateId);
            command.AddParam("@Title", SqlDbType.VarChar, post.Title);
            command.AddParam("@Summary", SqlDbType.VarChar, post.Summary);
            command.AddParam("@Description", SqlDbType.VarChar, post.Description);
            command.AddParam("@ImageCount", SqlDbType.Int, post.ImageCount);
            command.AddParam("@LikeCount", SqlDbType.Int, post.LikeCount);
            command.AddParam("@PublicationDateTime", SqlDbType.DateTime, post.PublicationDateTime);
            command.AddParam("@ApprovalDateTime", SqlDbType.DateTime, post.ApprovalDateTime);
            command.AddParam("@ExpirationDateTime", SqlDbType.DateTime, post.ExpirationDateTime);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, post.Status);

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

            command.AddParam("@AppUserId", SqlDbType.BigInt, post.AppUserId);
            command.AddParam("@PostTypeId", SqlDbType.BigInt, post.PostTypeId);
            command.AddParam("@CountryId", SqlDbType.BigInt, post.CountryId);
            command.AddParam("@StateId", SqlDbType.BigInt, post.StateId);
            command.AddParam("@Title", SqlDbType.VarChar, post.Title);
            command.AddParam("@Summary", SqlDbType.VarChar, post.Summary);
            command.AddParam("@Description", SqlDbType.VarChar, post.Description);
            command.AddParam("@ImageCount", SqlDbType.Int, post.ImageCount);
            command.AddParam("@LikeCount", SqlDbType.Int, post.LikeCount);
            command.AddParam("@PublicationDateTime", SqlDbType.DateTime, post.PublicationDateTime);
            command.AddParam("@ApprovalDateTime", SqlDbType.DateTime, post.ApprovalDateTime);
            command.AddParam("@ExpirationDateTime", SqlDbType.DateTime, post.ExpirationDateTime);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, post.Status);
            command.AddParam("@Id", SqlDbType.BigInt, post.Id);

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

            command.AddParam("@ImageCount", SqlDbType.Int, imageCount);
            command.AddParam("@Id", SqlDbType.BigInt, id);

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

            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DecrementLikeCount(long id)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET LikeCount = LikeCount - 1" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

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

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE Id = @Id AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);
            command.AddParam("@Id", SqlDbType.BigInt, id);

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
    }
}
