using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class NewsDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-News]";

        private static News GetNews(SqlDataReader reader)
        {
            return new News(Convert.ToInt64(reader["Id"]),
                            Convert.ToInt64(reader["PostId"]),
                            Convert.ToInt64(reader["NewsTypeId"]),
                            reader["Place"].ToString(),
                            reader["Source"].ToString(),
                            reader["DateTime"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateTime"]),
                            Convert.ToDateTime(reader["CreateDateTime"]),
                            Convert.ToDateTime(reader["UpdateDateTime"]),
                            Convert.ToInt32(reader["Status"]));
        }

        public static NewsFull GetNewsFull(SqlDataReader reader)
        {
            return new NewsFull(Convert.ToInt64(reader["Id"]),

                                Convert.ToInt64(reader["PostId"]),
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

                                new int[]{Convert.ToInt32(reader["ReactionCount1"]),
                                          Convert.ToInt32(reader["ReactionCount2"]),
                                          Convert.ToInt32(reader["ReactionCount3"]),
                                          Convert.ToInt32(reader["ReactionCount4"])},
                                Convert.ToInt32(reader["CommentCount"]),
                                
                                Convert.ToInt32(reader["Favorite"]),
                                Convert.ToInt32(reader["Like"]),
                                Convert.ToInt32(reader["LikeCount"]),
                                Convert.ToInt64(reader["ReactionPhraseId"]),
                                Convert.ToDateTime(reader["PublicationDateTime"]),
                                Convert.ToInt32(reader["PostStatus"]),

                                new AppUserInfo(Convert.ToInt64(reader["AppUserId"]),
                                                reader["AppUserAlias"].ToString(),
                                                null,

                                                new LocalityFull(Convert.ToInt64(reader["InterestLocalityTypeId"]),
                                                                 Convert.ToInt64(reader["InterestLocalityCountryId"]),
                                                                 Convert.ToInt64(reader["InterestLocalityStateId"]),
                                                                 Convert.ToInt64(reader["InterestLocalityCityId"])),

                                                new LocalityFull(Convert.ToInt64(reader["CurrentLocalityTypeId"]),
                                                                 Convert.ToInt64(reader["CurrentLocalityCountryId"]),
                                                                 Convert.ToInt64(reader["CurrentLocalityStateId"]),
                                                                 Convert.ToInt64(reader["CurrentLocalityCityId"]))),

                                null,   //ContactFull
                                null,   //LinkFulls
                                null,   //CommentFulls

                                Convert.ToInt64(reader["NewsTypeId"]),
                                reader["Place"].ToString(),
                                reader["Source"].ToString(),
                                reader["DateTime"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateTime"]),
                                Convert.ToInt32(reader["Status"]),
                                null);  //Images);
        }

        public static NewsFeed GetNewsFeed(SqlDataReader reader)
        {
            return new NewsFeed(Convert.ToInt64(reader["NewsId"]),
                                Convert.ToInt64(reader["PostId"]),
                                null,   //TitleImage
                                reader["Title"].ToString(),
                                reader["Description"].ToString(),
                                reader["DateTime"] == DBNull.Value ? null : Convert.ToDateTime(reader["DateTime"]),
                                reader["Source"].ToString(),
                                [Convert.ToInt32(reader["ReactionCount1"]), Convert.ToInt32(reader["ReactionCount2"]),
                                 Convert.ToInt32(reader["ReactionCount3"]), Convert.ToInt32(reader["ReactionCount4"])],
                                Convert.ToInt64(reader["ReactionPhraseId"]),
                                Convert.ToInt32(reader["CommentCount"]),
                                reader["Alias"].ToString());
        }

        // GET
        public async Task<List<News>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            List<News> newss = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         News news = GetNews(reader);
                         newss.Add(news);
                    }
                }
            }
            return newss;
        }

        public async Task<News> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            News news = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         news = GetNews(reader);
                    }
                }
            }
            return news;
        }

        // GET FEED
        public async Task<NewsFeedResponse> GetFeed(NewsFeedRequest request)
        {
            NewsFeedResponse response = new NewsFeedResponse(request);

            (String whereFeed, String whereCount) = PostDB.GetFeedWheres(PostType.News, request);

            // QUERY FEED
            String strCmd = PostDB.InitFeedCmd(request.Direction, "PublicationDateTime");

            strCmd += "SELECT Post.Id AS PostId," +
                      " News.Id AS NewsId," +
                      " Post.Title," +
                      " Post.Description," +
                      " News.DateTime," +
                      " News.Source," +
                      " ISNULL((SELECT COUNT(*) FROM[D-Reaction] AS Reaction WHERE Reaction.PostId = Post.Id AND Reaction.ReactionPhraseId = 1), 0) AS ReactionCount1," +
                      " ISNULL((SELECT COUNT(*) FROM[D-Reaction] AS Reaction WHERE Reaction.PostId = Post.Id AND Reaction.ReactionPhraseId = 2), 0) AS ReactionCount2," +
                      " ISNULL((SELECT COUNT(*) FROM[D-Reaction] AS Reaction WHERE Reaction.PostId = Post.Id AND Reaction.ReactionPhraseId = 3), 0) AS ReactionCount3," +
                      " ISNULL((SELECT COUNT(*) FROM[D-Reaction] AS Reaction WHERE Reaction.PostId = Post.Id AND Reaction.ReactionPhraseId = 4), 0) AS ReactionCount4," +
                      " ISNULL(Reaction.ReactionPhraseId, -1) AS ReactionPhraseId," +
                      " ISNULL((SELECT COUNT(*) FROM[D-Comment] AS Comment WHERE Comment.PostId = Post.Id AND Comment.Status = 1), 0) AS CommentCount," +
                      " AppUser.Alias" +
                      " FROM[D-Post] AS Post" +
                      " INNER JOIN[D-AppUser] AS AppUser ON Post.AppUserId = AppUser.Id" +
                      " INNER JOIN[D-News] AS News ON News.PostId = Post.Id" +
                      " LEFT JOIN[D-Reaction] AS Reaction ON Reaction.PostId = Post.Id AND Reaction.AppUserId = @ReactionAppUserId" +
                        whereFeed;

            strCmd += PostDB.OrderFeedCmd(request.Direction, "PublicationDateTime");

            // POST COUNT
            strCmd += "SELECT COUNT(*) AS Total FROM [D-Post] AS Post" + whereCount + ";";

            using (SqlCommand command = new SqlCommand(strCmd, conn))
            {
                command.AddFeedParams(request);

                using (conn)
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            response.NewsFeeds.Add(GetNewsFeed(reader));

                        await reader.NextResultAsync();
                        if (await reader.ReadAsync())
                            response.Total = Convert.ToInt32(reader["Total"]);
                    }
                }
            }

            return response;
        }

        // GET FULL
        public async Task<NewsFull> GetFullById(long id, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +

                             // ReactionCounts
                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 1), 0) AS ReactionCount1," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 2), 0) AS ReactionCount2," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 3), 0) AS ReactionCount3," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 4), 0) AS ReactionCount4," +

                             // CommentCount
                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Comment] AS Comment" +
                             "        WHERE Comment.PostId = Post.Id" +
                             "        AND Comment.Status = 1), 0) AS CommentCount," +

                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(DLike.[Rank], -1) AS [Like]," +
                             " ISNULL(DReaction.[ReactionPhraseId], -1) AS [ReactionPhraseId]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status AS PostStatus," +

                             // Interest Locality
                             " ISNULL(InterestLocality.LocalityType, -1) AS InterestLocalityTypeId," +
                             " ISNULL(InterestLocality.CountryId, -1) AS InterestLocalityCountryId," +
                             " ISNULL(InterestLocality.StateId, -1) AS InterestLocalityStateId," +
                             " ISNULL(InterestLocality.CityId, -1) AS InterestLocalityCityId," +

                             // Current Locality
                             " ISNULL(CurrentLocality.LocalityType, -1) AS CurrentLocalityTypeId," +
                             " ISNULL(CurrentLocality.CountryId, -1) AS CurrentLocalityCountryId," +
                             " ISNULL(CurrentLocality.StateId, -1) AS CurrentLocalityStateId," +
                             " ISNULL(CurrentLocality.CityId, -1) AS CurrentLocalityCityId," +

                            $" {table}.NewsTypeId, {table}.Place," +
                            $" {table}.Source, {table}.DateTime, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                             " INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                             " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Like] AS DLike ON DLike.PostId = Post.Id AND DLike.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Reaction] AS DReaction ON DReaction.PostId = Post.Id AND DReaction.AppUserId = @LikeAppUserId" +

                             // Interest locality
                             " OUTER APPLY (" +
                             "     SELECT TOP 1 LocalityType, CountryId, StateId, CityId" +
                             "     FROM [D-Locality]" +
                             "     WHERE AppUserId = Post.AppUserId" +
                             "     AND LocalityType = 1" +
                             "     AND Status = 1" +
                             " ) AS InterestLocality" +

                             // Current locality
                             " OUTER APPLY (" +
                             "     SELECT TOP 1 LocalityType, CountryId, StateId, CityId" +
                             "     FROM [D-Locality]" +
                             "     WHERE AppUserId = Post.AppUserId" +
                             "     AND LocalityType = 2" +
                             "     AND Status = 1" +
                             " ) AS CurrentLocality" +

                            $" WHERE {table}.Id = @Id;";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                      $" WHERE Status = 1 AND PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
                       " FROM [D-Link] AS Link" +
                      $" WHERE Link.Status = 1 AND Link.PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            strCmd += "SELECT TOP 3 Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " Comment.Message, Comment.PublicationDateTime, Comment.CreateDateTime, Comment.UpdateDateTime, Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON (Comment.AppUserId = AppUser.Id)" +
                     $" WHERE Comment.Status = 1 AND Comment.PostId = (SELECT PostId FROM {table} WHERE Id = @Id)" +
                      " ORDER BY Comment.PublicationDateTime DESC;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            command.AddParam("@Id", SqlDbType.BigInt, id);
            command.AddParam("@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

            NewsFull newsFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    newsFull = GetNewsFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        newsFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    newsFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    newsFull.CommentFulls = commentFulls;
                }
            }

            return newsFull;
        }

        public async Task<NewsFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +

                             // ReactionCounts
                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 1), 0) AS ReactionCount1," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 2), 0) AS ReactionCount2," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 3), 0) AS ReactionCount3," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 4), 0) AS ReactionCount4," +

                             // CommentCount
                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Comment] AS Comment" +
                             "        WHERE Comment.PostId = Post.Id" +
                             "        AND Comment.Status = 1), 0) AS CommentCount," +

                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(DLike.[Rank], -1) AS [Like]," +
                             " ISNULL(DReaction.[ReactionPhraseId], -1) AS [ReactionPhraseId]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status AS PostStatus," +

                             // Interest Locality
                             " ISNULL(InterestLocality.LocalityType, -1) AS InterestLocalityTypeId," +
                             " ISNULL(InterestLocality.CountryId, -1) AS InterestLocalityCountryId," +
                             " ISNULL(InterestLocality.StateId, -1) AS InterestLocalityStateId," +
                             " ISNULL(InterestLocality.CityId, -1) AS InterestLocalityCityId," +

                             // Current Locality
                             " ISNULL(CurrentLocality.LocalityType, -1) AS CurrentLocalityTypeId," +
                             " ISNULL(CurrentLocality.CountryId, -1) AS CurrentLocalityCountryId," +
                             " ISNULL(CurrentLocality.StateId, -1) AS CurrentLocalityStateId," +
                             " ISNULL(CurrentLocality.CityId, -1) AS CurrentLocalityCityId," +

                            $" {table}.NewsTypeId, {table}.Place," +
                            $" {table}.Source, {table}.DateTime, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                             " INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                             " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Like] AS DLike ON DLike.PostId = Post.Id AND DLike.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Reaction] AS DReaction ON DReaction.PostId = Post.Id AND DReaction.AppUserId = @LikeAppUserId" +

                             // Interest locality
                             " OUTER APPLY (" +
                             "     SELECT TOP 1 LocalityType, CountryId, StateId, CityId" +
                             "     FROM [D-Locality]" +
                             "     WHERE AppUserId = Post.AppUserId" +
                             "     AND LocalityType = 1" +
                             "     AND Status = 1" +
                             " ) AS InterestLocality" +

                             // Current locality
                             " OUTER APPLY (" +
                             "     SELECT TOP 1 LocalityType, CountryId, StateId, CityId" +
                             "     FROM [D-Locality]" +
                             "     WHERE AppUserId = Post.AppUserId" +
                             "     AND LocalityType = 2" +
                             "     AND Status = 1" +
                             " ) AS CurrentLocality" +

                            $" WHERE {table}.PostId = @PostId;";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                       " WHERE Status = 1 AND PostId = @PostId;";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
              " FROM [D-Link] AS Link" +
              " WHERE Link.Status = 1 AND Link.PostId = @PostId;";

            strCmd += "SELECT TOP 3 Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " Comment.Message, Comment.PublicationDateTime, Comment.CreateDateTime, Comment.UpdateDateTime, Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
                      " WHERE Comment.Status = 1 AND Comment.PostId = @PostId" +
                      " ORDER BY Comment.PublicationDateTime DESC;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            command.AddParam("@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

            NewsFull newsFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    newsFull = GetNewsFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        newsFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    newsFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    newsFull.CommentFulls = commentFulls;
                }
            }

            return newsFull;
        }

        public async Task<NewsDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +

                             // ReactionCounts
                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 1), 0) AS ReactionCount1," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 2), 0) AS ReactionCount2," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 3), 0) AS ReactionCount3," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 4), 0) AS ReactionCount4," +

                             // CommentCount
                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Comment] AS Comment" +
                             "        WHERE Comment.PostId = Post.Id" +
                             "        AND Comment.Status = 1), 0) AS CommentCount," +

                             " 0 AS Favorite, -1 AS [Like], Post.LikeCount, Post.PublicationDateTime, Post.Status AS PostStatus," +

                             // Interest Locality
                             " ISNULL(InterestLocality.LocalityType, -1) AS InterestLocalityTypeId," +
                             " ISNULL(InterestLocality.CountryId, -1) AS InterestLocalityCountryId," +
                             " ISNULL(InterestLocality.StateId, -1) AS InterestLocalityStateId," +
                             " ISNULL(InterestLocality.CityId, -1) AS InterestLocalityCityId," +

                             // Current Locality
                             " ISNULL(CurrentLocality.LocalityType, -1) AS CurrentLocalityTypeId," +
                             " ISNULL(CurrentLocality.CountryId, -1) AS CurrentLocalityCountryId," +
                             " ISNULL(CurrentLocality.StateId, -1) AS CurrentLocalityStateId," +
                             " ISNULL(CurrentLocality.CityId, -1) AS CurrentLocalityCityId," +

                            $" {table}.NewsTypeId, {table}.Place," +
                            $" {table}.Source, {table}.DateTime, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +

                             // Interest locality
                             " OUTER APPLY (" +
                             "     SELECT TOP 1 LocalityType, CountryId, StateId, CityId" +
                             "     FROM [D-Locality]" +
                             "     WHERE AppUserId = Post.AppUserId" +
                             "     AND LocalityType = 1" +
                             "     AND Status = 1" +
                             " ) AS InterestLocality" +

                             // Current locality
                             " OUTER APPLY (" +
                             "     SELECT TOP 1 LocalityType, CountryId, StateId, CityId" +
                             "     FROM [D-Locality]" +
                             "     WHERE AppUserId = Post.AppUserId" +
                             "     AND LocalityType = 2" +
                             "     AND Status = 1" +
                             " ) AS CurrentLocality";

            if (status != -1)
                strCmd += $" WHERE {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT Contact.Id, Contact.PostId, Contact.Name, Contact.Status" +
                      " FROM [D-Contact] AS Contact" +
                      $" INNER JOIN {table} ON (Contact.PostId = {table}.PostId)" +
                       " WHERE Contact.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
                       " FROM [D-Link] AS Link" +
                      $" INNER JOIN {table} ON (Link.PostId = {table}.PostId)" +
                       " WHERE Link.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT Id, PostId, AppUserId, AppUserAlias," +
                      " Message, PublicationDateTime, CreateDateTime, UpdateDateTime, Status" +
                      " FROM (" +
                      " SELECT Comment.Id, Comment.PostId, Comment.AppUserId," +
                      "        AppUser.Alias AS AppUserAlias," +
                      "        Comment.Message, Comment.PublicationDateTime, Comment.CreateDateTime," +
                      "        Comment.UpdateDateTime, Comment.Status," +
                      "        ROW_NUMBER() OVER (" +
                      "            PARTITION BY Comment.PostId" +
                      "            ORDER BY Comment.PublicationDateTime DESC" +
                      "        ) AS RowNumber" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
                     $" INNER JOIN {table} ON (Comment.PostId = {table}.PostId)" +
                      " WHERE Comment.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status";

            strCmd += ") AS Comments" +
                      " WHERE RowNumber <= 3" +
                      " ORDER BY PostId, PublicationDateTime DESC;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            NewsDataFull newsDataFull = new NewsDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<NewsFull> newsFulls = [];
                    while (await reader.ReadAsync())
                        newsFulls.Add(GetNewsFull(reader));
                    newsDataFull.NewsFulls = newsFulls;

                    await reader.NextResultAsync();
                    List<ContactFull> contactFulls = [];
                    while (await reader.ReadAsync())
                        contactFulls.Add(ContactDB.GetContactFull(reader));
                    newsDataFull.ContactFulls = contactFulls;

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    newsDataFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    newsDataFull.CommentFulls = commentFulls;
                }
            }

            return newsDataFull;
        }

        // INSERT
        public async Task<long> Add(News news)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, NewsTypeId, Place, Source, DateTime, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @NewsTypeId, @Place, @Source, @DateTime, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('N'));
            command.AddParam("@PostId", SqlDbType.BigInt, news.PostId);
            command.AddParam("@NewsTypeId", SqlDbType.BigInt, news.NewsTypeId);
            command.AddParam("@Place", SqlDbType.VarChar, news.Place);
            command.AddParam("@Source", SqlDbType.VarChar, news.Source);
            command.AddParam("@DateTime", SqlDbType.DateTime, news.DateTime);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, news.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(News news)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, NewsTypeId = @NewsTypeId, Place = @Place, Source = @Source, DateTime = @DateTime, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, news.PostId);
            command.AddParam("@NewsTypeId", SqlDbType.BigInt, news.NewsTypeId);
            command.AddParam("@Place", SqlDbType.VarChar, news.Place);
            command.AddParam("@Source", SqlDbType.VarChar, news.Source);
            command.AddParam("@DateTime", SqlDbType.DateTime, news.DateTime);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, news.Status);
            command.AddParam("@Id", SqlDbType.BigInt, news.Id);

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

        public async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE PostId = @PostId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);
            command.AddParam("@PostId", SqlDbType.BigInt, postId);

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

        public async Task<bool> DeleteByPostId(long postId)
        {
            String strCmd = $"DELETE {table} WHERE PostId = @PostId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
