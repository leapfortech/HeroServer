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
                                Convert.ToInt64(reader["PostCountryId"]),
                                Convert.ToInt64(reader["PostStateId"]),
                                reader["Title"].ToString(),
                                null, //TitleImage
                                reader["Summary"].ToString(),
                                reader["Description"].ToString(),
                                Convert.ToInt32(reader["ImageCount"]),

                                new int[]{Convert.ToInt32(reader["Reaction1Count"]),
                                          Convert.ToInt32(reader["Reaction2Count"]),
                                          Convert.ToInt32(reader["Reaction3Count"]),
                                          Convert.ToInt32(reader["Reaction4Count"])},
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

                                Convert.ToInt32(reader["Status"]),

                                null);  //Images
        }

        public static TaleFeed GetTaleFeed(SqlDataReader reader)
        {
            return new TaleFeed(Convert.ToInt64(reader["TaleId"]),
                                Convert.ToInt64(reader["PostId"]),
                                null,   //TitleImage
                                reader["Title"].ToString(),
                                reader["Description"].ToString(),
                                [Convert.ToInt32(reader["Reaction1Count"]),
                                 Convert.ToInt32(reader["Reaction2Count"]),
                                 Convert.ToInt32(reader["Reaction3Count"]),
                                 Convert.ToInt32(reader["Reaction4Count"])],
                                Convert.ToInt64(reader["ReactionPhraseId"]),
                                Convert.ToInt32(reader["CommentCount"]),
                                reader["Alias"].ToString(),
                                reader["InterestLocality"].ToString(),
                                reader["CurrentLocality"].ToString());
        }

        // GET
        public async Task<List<Tale>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            List<Tale> tales = [];
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

            command.AddParam("@Id", SqlDbType.BigInt, id);

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

        // GET FEED
        public async Task<TaleFeedResponse> GetFeed(TaleFeedRequest request)
        {
            TaleFeedResponse response = new TaleFeedResponse(request);

            (String whereFeed, String whereCount) = DBHelper.GetFeedWheres(request);

            // QUERY FEED
            String strCmd = DBHelper.InitFeedCmd(request.Direction, "PublicationDateTime");

            strCmd += "SELECT Post.Id AS PostId," +
                      " Tale.Id AS TaleId," +
                      " Post.Title," +
                      " Post.Description," +
                      " ISNULL((SELECT COUNT(*) FROM [D-Reaction] AS Reaction WHERE Reaction.PostId = Post.Id AND Reaction.ReactionPhraseId = 1), 0) AS Reaction1Count," +
                      " ISNULL((SELECT COUNT(*) FROM [D-Reaction] AS Reaction WHERE Reaction.PostId = Post.Id AND Reaction.ReactionPhraseId = 2), 0) AS Reaction2Count," +
                      " ISNULL((SELECT COUNT(*) FROM [D-Reaction] AS Reaction WHERE Reaction.PostId = Post.Id AND Reaction.ReactionPhraseId = 3), 0) AS Reaction3Count," +
                      " ISNULL((SELECT COUNT(*) FROM [D-Reaction] AS Reaction WHERE Reaction.PostId = Post.Id AND Reaction.ReactionPhraseId = 4), 0) AS Reaction4Count," +
                      " ISNULL(Reaction.ReactionPhraseId, -1) AS ReactionPhraseId," +
                      " ISNULL((SELECT COUNT(*) FROM [D-Comment] AS Comment WHERE Comment.PostId = Post.Id AND Comment.Status = 1), 0) AS CommentCount," +
                      " AppUser.Alias," +
                      " CASE" +
                      " WHEN InterestLocality.CountryId <> -1 AND InterestLocality.StateId <> -1 AND InterestLocality.CityId <> -1 THEN InterestLocalityCountry.Name + ', ' + InterestLocalityState.Name + ', ' + InterestLocalityCity.Name" +
                      " WHEN InterestLocality.CountryId <> -1 AND InterestLocality.StateId <> -1 THEN InterestLocalityCountry.Name + ', ' + InterestLocalityState.Name" +
                      " WHEN InterestLocality.CountryId <> -1 THEN InterestLocalityCountry.Name" +
                      " ELSE ''" +
                      " END AS InterestLocality," +
                      " CASE" +
                      " WHEN CurrentLocality.CountryId <> -1 AND CurrentLocality.StateId <> -1 AND CurrentLocality.CityId <> -1 THEN CurrentLocalityCountry.Name + ', ' + CurrentLocalityState.Name + ', ' + CurrentLocalityCity.Name" +
                      " WHEN CurrentLocality.CountryId <> -1 AND CurrentLocality.StateId <> -1 THEN CurrentLocalityCountry.Name + ', ' + CurrentLocalityState.Name" +
                      " WHEN CurrentLocality.CountryId <> -1 THEN CurrentLocalityCountry.Name" +
                      " ELSE ''" +
                      " END AS CurrentLocality" +
                      " FROM [D-Post] AS Post" +
                      " INNER JOIN [D-AppUser] AS AppUser ON Post.AppUserId = AppUser.Id" +
                      " INNER JOIN [D-Tale] AS Tale ON Tale.PostId = Post.Id" +
                      " LEFT JOIN [D-Reaction] AS Reaction ON Reaction.PostId = Post.Id AND Reaction.AppUserId = @ReactionAppUserId" +
                      " OUTER APPLY (SELECT TOP 1 LocalityType, CountryId, StateId, CityId FROM [D-Locality] WHERE AppUserId = Post.AppUserId AND LocalityType = 1 AND Status = 1) AS InterestLocality" +
                      " OUTER APPLY (SELECT TOP 1 LocalityType, CountryId, StateId, CityId FROM [D-Locality] WHERE AppUserId = Post.AppUserId AND LocalityType = 2 AND Status = 1) AS CurrentLocality" +
                      " LEFT JOIN [K-Country] AS InterestLocalityCountry ON InterestLocalityCountry.Id = InterestLocality.CountryId" +
                      " LEFT JOIN [K-State] AS InterestLocalityState ON InterestLocalityState.Id = InterestLocality.StateId" +
                      " LEFT JOIN [K-City] AS InterestLocalityCity ON InterestLocalityCity.Id = InterestLocality.CityId" +
                      " LEFT JOIN [K-Country] AS CurrentLocalityCountry ON CurrentLocalityCountry.Id = CurrentLocality.CountryId" +
                      " LEFT JOIN [K-State] AS CurrentLocalityState ON CurrentLocalityState.Id = CurrentLocality.StateId" +
                      " LEFT JOIN [K-City] AS CurrentLocalityCity ON CurrentLocalityCity.Id = CurrentLocality.CityId" +
                        whereFeed;

            strCmd += DBHelper.OrderFeedCmd(request.Direction, "PublicationDateTime");

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
                            response.TaleFeeds.Add(GetTaleFeed(reader));

                        await reader.NextResultAsync();
                        if (await reader.ReadAsync())
                            response.Total = Convert.ToInt32(reader["Total"]);
                    }
                }
            }

            return response;
        }

        // GET FULL
        public async Task<TaleFull> GetFullById(long id, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias," +
                             " Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId," +
                             " Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +

                             // ReactionCounts
                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 1), 0) AS Reaction1Count," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 2), 0) AS Reaction2Count," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 3), 0) AS Reaction3Count," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 4), 0) AS Reaction4Count," +

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

                            $" {table}.Status" +
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

            strCmd +=  "SELECT Id, PostId, Name, Status" +
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

            TaleFull taleFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    taleFull = GetTaleFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        taleFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    taleFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    taleFull.CommentFulls = commentFulls;
                }
            }

            return taleFull;
        }

        public async Task<TaleFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                            " Post.AppUserId, AppUser.Alias AS AppUserAlias," +
                            " Post.PostTypeId," +
                            " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                            " Post.ImageCount," +

                             // ReactionCounts
                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 1), 0) AS Reaction1Count," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 2), 0) AS Reaction2Count," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 3), 0) AS Reaction3Count," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 4), 0) AS Reaction4Count," +

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

                            $" {table}.Status" +
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

            strCmd +=  "SELECT Id, PostId, Name, Status" +
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

            TaleFull taleFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    taleFull = GetTaleFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        taleFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    taleFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    taleFull.CommentFulls = commentFulls;
                }
            }

            return taleFull;
        }

        public async Task<TaleDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +

                             // ReactionCounts
                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 1), 0) AS Reaction1Count," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 2), 0) AS Reaction2Count," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 3), 0) AS Reaction3Count," +

                             " ISNULL((SELECT COUNT(*)" +
                             "        FROM [D-Reaction] AS Reaction" +
                             "        WHERE Reaction.PostId = Post.Id" +
                             "        AND Reaction.ReactionPhraseId = 4), 0) AS Reaction4Count," +

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

                            $" {table}.Status" +
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


            TaleDataFull taleDataFull = new TaleDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<TaleFull> taleFulls = [];
                    while (await reader.ReadAsync())
                        taleFulls.Add(GetTaleFull(reader));
                    taleDataFull.TaleFulls = taleFulls;

                    await reader.NextResultAsync();
                    List<ContactFull> contactFulls = [];
                    while (await reader.ReadAsync())
                        contactFulls.Add(ContactDB.GetContactFull(reader));
                    taleDataFull.ContactFulls = contactFulls;

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    taleDataFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    taleDataFull.CommentFulls = commentFulls;
                }
            }

            return taleDataFull;
        }

        // INSERT
        public async Task<long> Add(Tale tale)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('T'));
            command.AddParam("@PostId", SqlDbType.BigInt, tale.PostId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, tale.Status);

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

            command.AddParam("@PostId", SqlDbType.BigInt, tale.PostId);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, tale.Status);
            command.AddParam("@Id", SqlDbType.BigInt, tale.Id);

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
