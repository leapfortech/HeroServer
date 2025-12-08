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

        private static Treatment GetTreatment(SqlDataReader reader)
        {
            return new Treatment(Convert.ToInt64(reader["Id"]),
                                 Convert.ToInt64(reader["PostId"]),
                                 reader["Ingredients"].ToString(),
                                 reader["Preparation"].ToString(),
                                 reader["Usage"].ToString(),
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
                                     Convert.ToInt64(reader["PostTypeId"]),
                                     Convert.ToInt64(reader["PostSubtypeId"]),
                                     Convert.ToInt64(reader["PostOriginCountryId"]),
                                     Convert.ToInt64(reader["PostOriginStateId"]),
                                     reader["Title"].ToString(),
                                     reader["Summary"].ToString(),
                                     reader["Description"].ToString(),
                                     Convert.ToInt32(reader["ImageCount"]),
                                     Convert.ToInt32(reader["LikeCount"]),
                                     Convert.ToDateTime(reader["PublicationDateTime"]),
                                     Convert.ToInt32(reader["PostStatus"]),

                                     reader["Ingredients"].ToString(),
                                     reader["Preparation"].ToString(),
                                     reader["Usage"].ToString(),
                                     Convert.ToInt32(reader["Status"]),

                                     null ); // DiseaseFulls
        }

        // GET
        public async Task<IEnumerable<Treatment>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Treatment> treatments = new List<Treatment>();
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
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId, Post.PostSubtypeId," +
                             " Post.PostOriginCountryId, Post.PostOriginStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.Ingredients, {table}.Preparation, {table}.Usage, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.Id = @Id;";

            strCmd +=  "SELECT Disease.Id, Disease.DiseaseTypeId, Disease.Status" +
                       " FROM [J-Disease] AS Disease" +
                      $" JOIN {table} ON (Disease.TreatmentId = {table}.Id)" +
                       " WHERE Disease.Status = 1" +
                      $" AND {table}.Id = @Id;";

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
                }
            }

            return treatmentFull;
        }

        public async Task<TreatmentFull> GetFullByPostId(long postId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId, Post.PostSubtypeId," +
                             " Post.PostOriginCountryId, Post.PostOriginStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.Ingredients, {table}.Preparation, {table}.Usage, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN Post ON ({table}.PostId = Post.PostId)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd +=  "SELECT Disease.Id, Disease.DiseaseTypeId, Disease.Status" +
                       " FROM [J-Disease] AS Disease" +
                      $" JOIN {table} ON (Disease.TreatmentId = {table}.Id)" +
                       " WHERE Disease.Status = 1" +
                      $" AND {table}.PostId = @PostId;";

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
                }
            }

            return treatmentFull;
        }

        public async Task<TreatmentDataFull> GetFullsByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId, Post.PostSubtypeId," +
                             " Post.PostOriginCountryId, Post.PostOriginStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                            $" {table}.Ingredients, {table}.Preparation, {table}.Usage, {table}.Status" +
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
                    {
                        TreatmentFull treatment = GetTreatmentFull(reader);
                        treatmentFulls.Add(treatment);
                    }
                    treatmentDataFull.TreatmentFulls = treatmentFulls;

                    await reader.NextResultAsync();
                    List<DiseaseFull> diseaseFulls = [];
                    while (await reader.ReadAsync())
                    {
                        DiseaseFull diseaseFull = DiseaseDB.GetDiseaseFull(reader);
                        diseaseFulls.Add(diseaseFull);
                    }

                    treatmentDataFull.DiseaseFulls = diseaseFulls;
                }
            }

            return treatmentDataFull;
        }

        // INSERT
        public async Task<long> Add(Treatment treatment)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, Ingredients, Preparation, Usage, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @Ingredients, @Preparation, @Usage, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, treatment.PostId);
            DBHelper.AddParam(command, "@Ingredients", SqlDbType.VarChar, treatment.Ingredients);
            DBHelper.AddParam(command, "@Preparation", SqlDbType.VarChar, treatment.Preparation);
            DBHelper.AddParam(command, "@Usage", SqlDbType.VarChar, treatment.Usage);
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
            String strCmd = $"UPDATE {table} SET PostId = @PostId, Ingredients = @Ingredients, Preparation = @Preparation, Usage = @Usage, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, treatment.PostId);
            DBHelper.AddParam(command, "@Ingredients", SqlDbType.VarChar, treatment.Ingredients);
            DBHelper.AddParam(command, "@Preparation", SqlDbType.VarChar, treatment.Preparation);
            DBHelper.AddParam(command, "@Usage", SqlDbType.VarChar, treatment.Usage);
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
