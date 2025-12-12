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
                                 Convert.ToInt64(reader["EventTypeId"]),
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
                                     Convert.ToInt64(reader["PostSubtypeId"]),
                                     Convert.ToInt64(reader["PostCountryId"]),
                                     Convert.ToInt64(reader["PostStateId"]),
                                     reader["Title"].ToString(),
                                     reader["Summary"].ToString(),
                                     reader["Description"].ToString(),
                                     Convert.ToInt32(reader["ImageCount"]),
                                     Convert.ToInt32(reader["LikeCount"]),
                                     Convert.ToDateTime(reader["PublicationDateTime"]),
                                     Convert.ToInt32(reader["PostStatus"]),
                                     null,   //ContactFull
                                     null,   //LinkFulls
                                     null,   //CommentFulls

                                     Convert.ToInt64(reader["EventTypeId"]),
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
                                      
                                     Convert.ToInt32(reader["Status"]));
        }


        // GET
        public async Task<List<Happening>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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
        public async Task<HappeningFull> GetFullById(long id)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.EventTypeId, {table}.CountryId, {table}.StateId, {table}.IsPublic, {table}.HasSignup," + 
                            $" {table}.HasPayment, {table}.PaymentDetails, {table}.StartDateTime, {table}.EndDateTime," +
                            $" {table}.Location, {table}.Latitude, {table}.Longitude, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.Id = @Id;";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                      $" WHERE Status = 1 AND PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
                       " FROM [D-Link] AS Link" +
                      $" WHERE Link.Status = 1 AND Link.PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON (Comment.AppUserId = AppUser.Id)" +
                     $" WHERE Comment.Status = 1 AND Comment.PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    happeningFull.CommentFulls = commentFulls;
                }
            }

            return happeningFull;
        }

        public async Task<HappeningFull> GetFullByPostId(long postId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.EventTypeId, {table}.CountryId, {table}.StateId, {table}.IsPublic, {table}.HasSignup," +
                            $" {table}.HasPayment, {table}.PaymentDetails, {table}.StartDateTime, {table}.EndDateTime," +
                            $" {table}.Location, {table}.Latitude, {table}.Longitude, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                       " WHERE Status = 1 AND PostId = @PostId;";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
              " FROM [D-Link] AS Link" +
              " WHERE Link.Status = 1 AND Link.PostId = @PostId;";

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
                      " WHERE Comment.Status = 1 AND Comment.PostId = @PostId;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);

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

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    happeningFull.CommentFulls = commentFulls;
                }
            }

            return happeningFull;
        }

        public async Task<HappeningDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.EventTypeId, {table}.CountryId, {table}.StateId, {table}.IsPublic, {table}.HasSignup," + 
                            $" {table}.HasPayment, {table}.PaymentDetails, {table}.StartDateTime, {table}.EndDateTime," +
                            $" {table}.Location, {table}.Latitude, {table}.Longitude, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
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

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                       " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                       " FROM [D-Comment] AS Comment" +
                       " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
                      $" INNER JOIN {table}" +
                      $" ON (Comment.PostId = {table}.PostId)" +
                       " WHERE Comment.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);


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

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    happeningDataFull.CommentFulls = commentFulls;
                }
            }

            return happeningDataFull;
        }

        // INSERT
        public async Task<long> Add(Happening happening)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, EventTypeId, CountryId, StateId, IsPublic, HasSignup, HasPayment, PaymentDetails, StartDateTime, EndDateTime, Location, Latitude, Longitude, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @EventTypeId, @CountryId, @StateId, @IsPublic, @HasSignup, @HasPayment, @PaymentDetails, @StartDateTime, @EndDateTime, @Location, @Latitude, @Longitude, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, happening.PostId);
            DBHelper.AddParam(command, "@EventTypeId", SqlDbType.BigInt, happening.EventTypeId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, happening.CountryId);
            DBHelper.AddParam(command, "@StateId", SqlDbType.BigInt, happening.StateId);
            DBHelper.AddParam(command, "@IsPublic", SqlDbType.Int, happening.IsPublic);
            DBHelper.AddParam(command, "@HasSignup", SqlDbType.Int, happening.HasSignup);
            DBHelper.AddParam(command, "@HasPayment", SqlDbType.Int, happening.HasPayment);
            DBHelper.AddParam(command, "@PaymentDetails", SqlDbType.VarChar, happening.PaymentDetails);
            DBHelper.AddParam(command, "@StartDateTime", SqlDbType.DateTime, happening.StartDateTime);
            DBHelper.AddParam(command, "@EndDateTime", SqlDbType.DateTime, happening.EndDateTime);
            DBHelper.AddParam(command, "@Location", SqlDbType.VarChar, happening.Location);
            DBHelper.AddParam(command, "@Latitude", SqlDbType.Decimal, happening.Latitude);
            DBHelper.AddParam(command, "@Longitude", SqlDbType.Decimal, happening.Longitude);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, happening.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Happening happening)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, EventTypeId = @EventTypeId, CountryId = @CountryId, StateId = @StateId, IsPublic = @IsPublic, HasSignup = @HasSignup, HasPayment = @HasPayment, PaymentDetails = @PaymentDetails, StartDateTime = @StartDateTime, EndDateTime = @EndDateTime, Location = @Location, Latitude = @Latitude, Longitude = @Longitude, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, happening.PostId);
            DBHelper.AddParam(command, "@EventTypeId", SqlDbType.BigInt, happening.EventTypeId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, happening.CountryId);
            DBHelper.AddParam(command, "@StateId", SqlDbType.BigInt, happening.StateId);
            DBHelper.AddParam(command, "@IsPublic", SqlDbType.Int, happening.IsPublic);
            DBHelper.AddParam(command, "@HasSignup", SqlDbType.Int, happening.HasSignup);
            DBHelper.AddParam(command, "@HasPayment", SqlDbType.Int, happening.HasPayment);
            DBHelper.AddParam(command, "@PaymentDetails", SqlDbType.VarChar, happening.PaymentDetails);
            DBHelper.AddParam(command, "@StartDateTime", SqlDbType.DateTime, happening.StartDateTime);
            DBHelper.AddParam(command, "@EndDateTime", SqlDbType.DateTime, happening.EndDateTime);
            DBHelper.AddParam(command, "@Location", SqlDbType.VarChar, happening.Location);
            DBHelper.AddParam(command, "@Latitude", SqlDbType.Decimal, happening.Latitude);
            DBHelper.AddParam(command, "@Longitude", SqlDbType.Decimal, happening.Longitude);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, happening.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, happening.Id);

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
