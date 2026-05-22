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
                                Convert.ToInt32(reader["Favorite"]),
                                Convert.ToInt32(reader["Like"]),
                                Convert.ToInt32(reader["LikeCount"]),
                                Convert.ToDateTime(reader["PublicationDateTime"]),
                                Convert.ToInt32(reader["Status"]),
                                null,   //ContactFull
                                null,   //LinkFulls
                                null,   //CommentFulls

                                Convert.ToInt32(reader["Status"]),

                                null);  //Images
        }

        // GET
        public async Task<List<Tale>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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
        public async Task<TaleFull> GetFullById(long id, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias," +
                             " Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId," +
                             " Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(Lik.[Rank], -1) AS [Like]," +                             
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId " +
                            " LEFT JOIN [D-Like] AS Lik ON Lik.PostId = Post.Id AND Lik.AppUserId = @LikeAppUserId " +
                            $" WHERE {table}.Id = @Id;";

            strCmd +=  "SELECT Id, PostId, Name, Status" +
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
            DBHelper.AddParam(command, "@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

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
                            " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                            " ISNULL(Lik.[Rank], -1) AS [Like]," +
                            " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId " +
                            " LEFT JOIN [D-Like] AS Lik ON Lik.PostId = Post.Id AND Lik.AppUserId = @LikeAppUserId " +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd +=  "SELECT Id, PostId, Name, Status" +
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
            DBHelper.AddParam(command, "@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

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
                             " Post.ImageCount, 0 AS Favorite, -1 AS [Like], Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.Status" +
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

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('T'));
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

        public async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE PostId = @PostId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);

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

        public async Task<bool> DeleteByPostId(long postId)
        {
            String strCmd = $"DELETE {table} WHERE PostId = @PostId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
