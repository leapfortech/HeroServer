using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class RadioDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Radio]";

        private static Radio GetRadio(SqlDataReader reader)
        {
            return new Radio(Convert.ToInt64(reader["Id"]),
                             Convert.ToInt64(reader["PostId"]),
                             Convert.ToDateTime(reader["CreateDateTime"]),
                             Convert.ToDateTime(reader["UpdateDateTime"]),
                             Convert.ToInt32(reader["Status"]));
        }

        public static RadioFull GetRadioFull(SqlDataReader reader)
        {
            return new RadioFull(Convert.ToInt64(reader["Id"]),

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

                                Convert.ToInt32(reader["Status"]),
                                null,   //RadioTypeFulls 
                                null,   //RadioLanguageFulls

                                null);   //Images);
        }


        // GET
        public async Task<List<Radio>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            List<Radio> radios = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Radio radio = GetRadio(reader);
                         radios.Add(radio);
                    }
                }
            }
            return radios;
        }

        public async Task<Radio> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            Radio radio = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         radio = GetRadio(reader);
                    }
                }
            }
            return radio;
        }

        public async Task<long> GetIdByPostId(long postId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE PostId = @PostId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, postId);

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

        // GET FULL
        public async Task<RadioFull> GetFullById(long id, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(DLike.[Rank], -1) AS [Like]," +
                             " ISNULL(DReaction.[ReactionPhraseId], -1) AS [ReactionPhraseId]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                             " INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                             " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Like] AS DLike ON DLike.PostId = Post.Id AND DLike.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Reaction] AS DReaction ON DReaction.PostId = Post.Id AND DReaction.AppUserId = @LikeAppUserId" +
                            $" WHERE {table}.Id = @Id;";

            strCmd += "SELECT Id, RadioTypeId, Status" +
                      " FROM [J-RadioType]" +
                      " WHERE Status = 1" +
                      " AND RadioId = @Id;";

            strCmd += "SELECT Id, LanguageId, Status" +
                      " FROM [J-RadioLanguage]" +
                      " WHERE Status = 1" +
                      " AND RadioId = @Id;";

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

            RadioFull radioFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    radioFull = GetRadioFull(reader);

                    await reader.NextResultAsync();
                    radioFull.RadioTypeFulls = [];
                    while (await reader.ReadAsync())
                        radioFull.RadioTypeFulls.Add(RadioTypeDB.GetRadioTypeFull(reader));

                    await reader.NextResultAsync();
                    radioFull.RadioLanguageFulls = [];
                    while (await reader.ReadAsync())
                        radioFull.RadioLanguageFulls.Add(RadioLanguageDB.GetRadioLanguageFull(reader));

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        radioFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    radioFull.LinkFulls = linkFulls;

                    //await reader.NextResultAsync();
                    //List<CommentFull> commentFulls = [];
                    //while (await reader.ReadAsync())
                    //    commentFulls.Add(CommentDB.GetCommentFull(reader));
                    //radioFull.CommentFulls = commentFulls;
                }
            }

            return radioFull;
        }

        public async Task<RadioFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(Lik.[Rank], -1) AS [Like]," +
                             " ISNULL(DReaction.[ReactionPhraseId], -1) AS [ReactionPhraseId]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                             " INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                             " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Like] AS Lik ON Lik.PostId = Post.Id AND Lik.AppUserId = @LikeAppUserId" +
                             " LEFT JOIN [D-Reaction] AS DReaction ON DReaction.PostId = Post.Id AND DReaction.AppUserId = @LikeAppUserId" +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd += "SELECT Id, RadioTypeId, Status" +
                      " FROM [J-RadioType]" +
                      " WHERE Status = 1" +
                      " AND RadioId IN" +
                      $" (SELECT Id FROM {table} WHERE PostId = @PostId);";

            strCmd += "SELECT Id, LanguageId, Status" +
                      " FROM [J-RadioLanguage]" +
                      " WHERE Status = 1" +
                      " AND RadioId IN" +
                      $" (SELECT Id FROM {table} WHERE PostId = @PostId);";

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

            RadioFull radioFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    radioFull = GetRadioFull(reader);

                    await reader.NextResultAsync();
                    radioFull.RadioTypeFulls = [];
                    while (await reader.ReadAsync())
                        radioFull.RadioTypeFulls.Add(RadioTypeDB.GetRadioTypeFull(reader));

                    await reader.NextResultAsync();
                    radioFull.RadioLanguageFulls = [];
                    while (await reader.ReadAsync())
                        radioFull.RadioLanguageFulls.Add(RadioLanguageDB.GetRadioLanguageFull(reader));

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        radioFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    radioFull.LinkFulls = linkFulls;

                    //await reader.NextResultAsync();
                    //List<CommentFull> commentFulls = [];
                    //while (await reader.ReadAsync())
                    //    commentFulls.Add(CommentDB.GetCommentFull(reader));
                    //radioFull.CommentFulls = commentFulls;
                }
            }

            return radioFull;
        }

        public async Task<RadioDataFull> GetDataFullByStatus(int status)
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

            strCmd += "SELECT RadioType.Id, RadioType.RadioTypeId, RadioType.Status" +
                       " FROM [J-RadioType] AS RadioType" +
                      $" JOIN {table} ON (RadioType.RadioId = {table}.Id)" +
                       " WHERE 1 = 1 AND RadioType.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT RadioLanguage.Id, RadioLanguage.LanguageId, RadioLanguage.Status" +
                       " FROM [J-RadioLanguage] AS RadioLanguage" +
                      $" JOIN {table} ON (RadioLanguage.RadioId = {table}.Id)" +
                       " WHERE 1 = 1 AND RadioLanguage.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
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

            RadioDataFull radioDataFull = new RadioDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<RadioFull> radioFulls = [];
                    while (await reader.ReadAsync())
                        radioFulls.Add(GetRadioFull(reader));
                    radioDataFull.RadioFulls = radioFulls;

                    await reader.NextResultAsync();
                    List<RadioTypeFull> radioTypeFulls = [];
                    while (await reader.ReadAsync())
                        radioTypeFulls.Add(RadioTypeDB.GetRadioTypeFull(reader));
                    radioDataFull.RadioTypeFulls = radioTypeFulls;

                    await reader.NextResultAsync();
                    List<RadioLanguageFull> radioLanguageFulls = [];
                    while (await reader.ReadAsync())
                        radioLanguageFulls.Add(RadioLanguageDB.GetRadioLanguageFull(reader));
                    radioDataFull.RadioLanguageFulls = radioLanguageFulls;

                    await reader.NextResultAsync();
                    List<ContactFull> contactFulls = [];
                    while (await reader.ReadAsync())
                        contactFulls.Add(ContactDB.GetContactFull(reader));
                    radioDataFull.ContactFulls = contactFulls;

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    radioDataFull.LinkFulls = linkFulls;

                    //await reader.NextResultAsync();
                    //List<CommentFull> commentFulls = [];
                    //while (await reader.ReadAsync())
                    //    commentFulls.Add(CommentDB.GetCommentFull(reader));
                    //radioDataFull.CommentFulls = commentFulls;
                }
            }

            return radioDataFull;
        }

        // INSERT
        public async Task<long> Add(Radio radio)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('R'));
            command.AddParam("@PostId", SqlDbType.BigInt, radio.PostId);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, radio.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Radio radio)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, radio.PostId);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, radio.Status);
            command.AddParam("@Id", SqlDbType.BigInt, radio.Id);

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
