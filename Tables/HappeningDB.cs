using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class HappeningDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Happening]";

        private static Happening GetHappening(SqlDataReader reader)
        {
            return new Happening(Convert.ToInt64(reader["Id"]),
                                 Convert.ToInt64(reader["PostId"]),
                                 Convert.ToInt64(reader["HappeningTypeId"]),
                                 Convert.ToInt64(reader["CountryId"]),
                                 Convert.ToInt64(reader["StateId"]),
                                 Convert.ToInt32(reader["IsPublic"]),
                                 Convert.ToInt32(reader["HasSignup"]),
                                 Convert.ToInt32(reader["HasPayment"]),
                                 reader["PaymentDetails"].ToString(),
                                 reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["StartDateTime"]),
                                 reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EndDateTime"]),
                                 reader["Location"].ToString(),
                                 reader["Latitude"] == DBNull.Value ? null : (double?)Convert.ToDouble(reader["Latitude"]),
                                 reader["Longitude"] == DBNull.Value ? null : (double?)Convert.ToDouble(reader["Longitude"]),
                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                 Convert.ToDateTime(reader["UpdateDateTime"]),
                                 Convert.ToInt32(reader["Status"]));
        }

        public static HappeningFull GetHappeningFull(SqlDataReader reader)
        {
            return new HappeningFull(Convert.ToInt64(reader["Id"]),

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
                                     Convert.ToInt32(reader["Favorite"]),
                                     Convert.ToInt32(reader["Like"]),
                                     Convert.ToInt32(reader["LikeCount"]),
                                     Convert.ToInt64(reader["ReactionPhraseId"]),
                                     Convert.ToDateTime(reader["PublicationDateTime"]),
                                     Convert.ToInt32(reader["Status"]),
                                     null,   //ContactFull
                                     null,   //LinkFulls
                                     null,   //CommentFulls

                                     Convert.ToInt64(reader["HappeningTypeId"]),
                                     Convert.ToInt64(reader["CountryId"]),
                                     Convert.ToInt64(reader["StateId"]),
                                     Convert.ToInt32(reader["IsPublic"]),
                                     Convert.ToInt32(reader["HasSignup"]),
                                     Convert.ToInt32(reader["HasPayment"]),
                                     reader["PaymentDetails"].ToString(),
                                     reader["StartDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["StartDateTime"]),
                                     reader["EndDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EndDateTime"]),
                                     reader["Location"].ToString(),
                                     reader["Latitude"] == DBNull.Value ? (double?)null : Convert.ToDouble(reader["Latitude"]),
                                     reader["Longitude"] == DBNull.Value ? (double?)null : Convert.ToDouble(reader["Longitude"]),
                                      
                                     Convert.ToInt32(reader["Status"]),

                                     null);  //Images)
        }


        // GET
        public async Task<List<Happening>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            List<Happening> happenings = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Happening happening = GetHappening(reader);
                         happenings.Add(happening);
                    }
                }
            }
            return happenings;
        }

        public async Task<Happening> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            Happening happening = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         happening = GetHappening(reader);
                    }
                }
            }
            return happening;
        }

        // GET FULL
        public async Task<HappeningFull> GetFullById(long id, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(DLike.[Rank], -1) AS [Like]," +
                             " ISNULL(DReaction.[ReactionPhraseId], -1) AS [ReactionPhraseId]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.HappeningTypeId, {table}.CountryId, {table}.StateId, {table}.IsPublic, {table}.HasSignup," + 
                            $" {table}.HasPayment, {table}.PaymentDetails, {table}.StartDateTime, {table}.EndDateTime," +
                            $" {table}.Location, {table}.Latitude, {table}.Longitude, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                             " INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                             " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Like] AS DLike ON DLike.PostId = Post.Id AND DLike.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Reaction] AS DReaction ON DReaction.PostId = Post.Id AND DReaction.AppUserId = @LikeAppUserId" +
                            $" WHERE {table}.Id = @Id;";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                      $" WHERE Status = 1 AND PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
                       " FROM [D-Link] AS Link" +
                      $" WHERE Link.Status = 1 AND Link.PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            //strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
            //          " Comment.Message, Comment.CreateDateTime, Comment.UpdateDateTime, Comment.Status" +
            //          " FROM [D-Comment] AS Comment" +
            //          " INNER JOIN [D-AppUser] AS AppUser ON (Comment.AppUserId = AppUser.Id)" +
            //         $" WHERE Comment.Status = 1 AND Comment.PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            SqlCommand command = new SqlCommand(strCmd, conn);
            command.AddParam("@Id", SqlDbType.BigInt, id);
            command.AddParam("@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

            HappeningFull happeningFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    happeningFull = GetHappeningFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        happeningFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    happeningFull.LinkFulls = linkFulls;

                    //await reader.NextResultAsync();
                    //List<CommentFull> commentFulls = [];
                    //while (await reader.ReadAsync())
                    //    commentFulls.Add(CommentDB.GetCommentFull(reader));
                    //happeningFull.CommentFulls = commentFulls;
                }
            }

            return happeningFull;
        }

        public async Task<HappeningFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(DLike.[Rank], -1) AS [Like]," +
                             " ISNULL(DReaction.[ReactionPhraseId], -1) AS [ReactionPhraseId]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.HappeningTypeId, {table}.CountryId, {table}.StateId, {table}.IsPublic, {table}.HasSignup," +
                            $" {table}.HasPayment, {table}.PaymentDetails, {table}.StartDateTime, {table}.EndDateTime," +
                            $" {table}.Location, {table}.Latitude, {table}.Longitude, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                             " INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                             " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId " +
                             " LEFT JOIN [D-Like] AS DLike ON DLike.PostId = Post.Id AND DLike.AppUserId = @LikeAppUserId " +
                             " LEFT JOIN [D-Reaction] AS DReaction ON DReaction.PostId = Post.Id AND DReaction.AppUserId = @LikeAppUserId" +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                       " WHERE Status = 1 AND PostId = @PostId;";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
              " FROM [D-Link] AS Link" +
              " WHERE Link.Status = 1 AND Link.PostId = @PostId;";

            //strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
            //          " Comment.Message, Comment.CreateDateTime, Comment.UpdateDateTime, Comment.Status" +
            //          " FROM [D-Comment] AS Comment" +
            //          " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
            //          " WHERE Comment.Status = 1 AND Comment.PostId = @PostId;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            command.AddParam("@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

            HappeningFull happeningFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    happeningFull = GetHappeningFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        happeningFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    happeningFull.LinkFulls = linkFulls;

                    //await reader.NextResultAsync();
                    //List<CommentFull> commentFulls = [];
                    //while (await reader.ReadAsync())
                    //    commentFulls.Add(CommentDB.GetCommentFull(reader));
                    //happeningFull.CommentFulls = commentFulls;
                }
            }

            return happeningFull;
        }

        public async Task<HappeningDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, 0 AS Favorite, -1 AS [Like], Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.HappeningTypeId, {table}.CountryId, {table}.StateId, {table}.IsPublic, {table}.HasSignup," + 
                            $" {table}.HasPayment, {table}.PaymentDetails, {table}.StartDateTime, {table}.EndDateTime," +
                            $" {table}.Location, {table}.Latitude, {table}.Longitude, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)";

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

            //strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
            //           " Comment.Message, Comment.CreateDateTime, Comment.UpdateDateTime, Comment.Status" +
            //           " FROM [D-Comment] AS Comment" +
            //           " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
            //          $" INNER JOIN {table}" +
            //          $" ON (Comment.PostId = {table}.PostId)" +
            //           " WHERE Comment.Status = 1";

            //if (status != -1)
            //    strCmd += $" AND {table}.Status = @Status;";
            //else
            //    strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);


            HappeningDataFull happeningDataFull = new HappeningDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<HappeningFull> happeningFulls = [];
                    while (await reader.ReadAsync())
                        happeningFulls.Add(GetHappeningFull(reader));
                    happeningDataFull.HappeningFulls = happeningFulls;

                    await reader.NextResultAsync();
                    List<ContactFull> contactFulls = [];
                    while (await reader.ReadAsync())
                        contactFulls.Add(ContactDB.GetContactFull(reader));
                    happeningDataFull.ContactFulls = contactFulls;

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    happeningDataFull.LinkFulls = linkFulls;

                    //await reader.NextResultAsync();
                    //List<CommentFull> commentFulls = [];
                    //while (await reader.ReadAsync())
                    //    commentFulls.Add(CommentDB.GetCommentFull(reader));
                    //happeningDataFull.CommentFulls = commentFulls;
                }
            }

            return happeningDataFull;
        }

        // INSERT
        public async Task<long> Add(Happening happening)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, HappeningTypeId, CountryId, StateId, IsPublic, HasSignup, HasPayment, PaymentDetails, StartDateTime, EndDateTime, Location, Latitude, Longitude, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @HappeningTypeId, @CountryId, @StateId, @IsPublic, @HasSignup, @HasPayment, @PaymentDetails, @StartDateTime, @EndDateTime, @Location, @Latitude, @Longitude, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('H'));
            command.AddParam("@PostId", SqlDbType.BigInt, happening.PostId);
            command.AddParam("@HappeningTypeId", SqlDbType.BigInt, happening.HappeningTypeId);
            command.AddParam("@CountryId", SqlDbType.BigInt, happening.CountryId);
            command.AddParam("@StateId", SqlDbType.BigInt, happening.StateId);
            command.AddParam("@IsPublic", SqlDbType.Int, happening.IsPublic);
            command.AddParam("@HasSignup", SqlDbType.Int, happening.HasSignup);
            command.AddParam("@HasPayment", SqlDbType.Int, happening.HasPayment);
            command.AddParam("@PaymentDetails", SqlDbType.VarChar, happening.PaymentDetails);
            command.AddParam("@StartDateTime", SqlDbType.DateTime, happening.StartDateTime);
            command.AddParam("@EndDateTime", SqlDbType.DateTime, happening.EndDateTime);
            command.AddParam("@Location", SqlDbType.VarChar, happening.Location);
            command.AddParam("@Latitude", SqlDbType.Decimal, happening.Latitude);
            command.AddParam("@Longitude", SqlDbType.Decimal, happening.Longitude);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, happening.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Happening happening)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, HappeningTypeId = @HappeningTypeId, CountryId = @CountryId, StateId = @StateId, IsPublic = @IsPublic, HasSignup = @HasSignup, HasPayment = @HasPayment, PaymentDetails = @PaymentDetails, StartDateTime = @StartDateTime, EndDateTime = @EndDateTime, Location = @Location, Latitude = @Latitude, Longitude = @Longitude, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, happening.PostId);
            command.AddParam("@HappeningTypeId", SqlDbType.BigInt, happening.HappeningTypeId);
            command.AddParam("@CountryId", SqlDbType.BigInt, happening.CountryId);
            command.AddParam("@StateId", SqlDbType.BigInt, happening.StateId);
            command.AddParam("@IsPublic", SqlDbType.Int, happening.IsPublic);
            command.AddParam("@HasSignup", SqlDbType.Int, happening.HasSignup);
            command.AddParam("@HasPayment", SqlDbType.Int, happening.HasPayment);
            command.AddParam("@PaymentDetails", SqlDbType.VarChar, happening.PaymentDetails);
            command.AddParam("@StartDateTime", SqlDbType.DateTime, happening.StartDateTime);
            command.AddParam("@EndDateTime", SqlDbType.DateTime, happening.EndDateTime);
            command.AddParam("@Location", SqlDbType.VarChar, happening.Location);
            command.AddParam("@Latitude", SqlDbType.Decimal, happening.Latitude);
            command.AddParam("@Longitude", SqlDbType.Decimal, happening.Longitude);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, happening.Status);
            command.AddParam("@Id", SqlDbType.BigInt, happening.Id);

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
