using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class TreatmentDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Treatment]";

        public static Treatment GetTreatment(SqlDataReader reader)
        {
            return new Treatment(Convert.ToInt64(reader["Id"]),
                                 Convert.ToInt64(reader["PostId"]),
                                 reader["Ingredients"].ToString(),
                                 reader["Preparation"].ToString(),
                                 reader["Usage"].ToString(),
                                 reader["Annotation"].ToString(),
                                 Convert.ToDateTime(reader["CreateDateTime"]),
                                 Convert.ToDateTime(reader["UpdateDateTime"]),
                                 Convert.ToInt32(reader["Status"]));
        }

        public static TreatmentFull GetTreatmentFull(SqlDataReader reader)
        {
            return new TreatmentFull(Convert.ToInt64(reader["Id"]),

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

                                     reader["Ingredients"].ToString(),
                                     reader["Preparation"].ToString(),
                                     reader["Usage"].ToString(),
                                     reader["Annotation"].ToString(),
                                     Convert.ToInt32(reader["Status"]),

                                     null ); // DiseaseFulls
        }

        // GET
        public async Task<List<Treatment>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Treatment> treatments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Treatment treatment = GetTreatment(reader);
                         treatments.Add(treatment);
                    }
                }
            }
            return treatments;
        }

        public async Task<Treatment> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Treatment treatment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         treatment = GetTreatment(reader);
                    }
                }
            }
            return treatment;
        }

        // GET FULL
        public async Task<TreatmentFull> GetFullById(long id)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.Ingredients, {table}.Preparation, {table}.Usage, {table}.Annotation, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.Id = @Id;";

            strCmd +=  "SELECT Disease.Id, Disease.DiseaseTypeId, Disease.Status" +
                       " FROM [J-Disease] AS Disease" +
                      $" JOIN {table} ON (Disease.TreatmentId = {table}.Id)" +
                       " WHERE Disease.Status = 1" +
                      $" AND {table}.Id = @Id;";

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

            TreatmentFull treatmentFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    treatmentFull = GetTreatmentFull(reader);

                    await reader.NextResultAsync();
                    treatmentFull.DiseaseFulls = [];
                    while (await reader.ReadAsync())
                    {
                        DiseaseFull diseaseFull = DiseaseDB.GetDiseaseFull(reader);
                        treatmentFull.DiseaseFulls.Add(diseaseFull);
                    }

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        treatmentFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    treatmentFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    treatmentFull.CommentFulls = commentFulls;
                }
            }

            return treatmentFull;
        }

        public async Task<TreatmentFull> GetFullByPostId(long postId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.Ingredients, {table}.Preparation, {table}.Usage, {table}.Annotation, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd +=  "SELECT Disease.Id, Disease.DiseaseTypeId, Disease.Status" +
                       " FROM [J-Disease] AS Disease" +
                      $" JOIN {table} ON (Disease.TreatmentId = {table}.Id)" +
                       " WHERE Disease.Status = 1" +
                      $" AND {table}.PostId = @PostId;";

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

            TreatmentFull treatmentFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    treatmentFull = GetTreatmentFull(reader);

                    await reader.NextResultAsync();
                    treatmentFull.DiseaseFulls = [];
                    while (await reader.ReadAsync())
                    {
                        DiseaseFull diseaseFull = DiseaseDB.GetDiseaseFull(reader);
                        treatmentFull.DiseaseFulls.Add(diseaseFull);
                    }

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        treatmentFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    treatmentFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    treatmentFull.CommentFulls = commentFulls;
                }
            }

            return treatmentFull;
        }

        public async Task<TreatmentDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.Ingredients, {table}.Preparation, {table}.Usage, {table}.Annotation, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)";

            if (status != -1)
                strCmd += $" WHERE {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd +=  "SELECT Disease.Id, Disease.DiseaseTypeId, Disease.Status, Disease.TreatmentId" +
                       " FROM [J-Disease] AS Disease" +
                      $" JOIN {table} ON (Disease.TreatmentId = {table}.Id)" +
                       " WHERE Disease.Status = 1";

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

            TreatmentDataFull treatmentDataFull = new TreatmentDataFull();
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<TreatmentFull> treatmentFulls = [];
                    while (await reader.ReadAsync())
                        treatmentFulls.Add(GetTreatmentFull(reader));
                    treatmentDataFull.TreatmentFulls = treatmentFulls;

                    await reader.NextResultAsync();
                    List<DiseaseFull> diseaseFulls = [];
                    while (await reader.ReadAsync())
                        diseaseFulls.Add(DiseaseDB.GetDiseaseFull(reader));
                    treatmentDataFull.DiseaseFulls = diseaseFulls;

                    await reader.NextResultAsync();
                    List<ContactFull> contactFulls = [];
                    while (await reader.ReadAsync())
                        contactFulls.Add(ContactDB.GetContactFull(reader));
                    treatmentDataFull.ContactFulls = contactFulls;

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    treatmentDataFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    treatmentDataFull.CommentFulls = commentFulls;
                }
            }

            return treatmentDataFull;
        }

        // INSERT
        public async Task<long> Add(Treatment treatment)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, Ingredients, Preparation, Usage, Annotation, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @Ingredients, @Preparation, @Usage, @Annotation, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, treatment.PostId);
            DBHelper.AddParam(command, "@Ingredients", SqlDbType.VarChar, treatment.Ingredients);
            DBHelper.AddParam(command, "@Preparation", SqlDbType.VarChar, treatment.Preparation);
            DBHelper.AddParam(command, "@Usage", SqlDbType.VarChar, treatment.Usage);
            DBHelper.AddParam(command, "@Annotation", SqlDbType.VarChar, treatment.Annotation);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, treatment.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Treatment treatment)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, Ingredients = @Ingredients, Preparation = @Preparation, Usage = @Usage, Annotation = @Annotation, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, treatment.PostId);
            DBHelper.AddParam(command, "@Ingredients", SqlDbType.VarChar, treatment.Ingredients);
            DBHelper.AddParam(command, "@Preparation", SqlDbType.VarChar, treatment.Preparation);
            DBHelper.AddParam(command, "@Usage", SqlDbType.VarChar, treatment.Usage);
            DBHelper.AddParam(command, "@Annotation", SqlDbType.VarChar, treatment.Annotation);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, treatment.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, treatment.Id);

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
