using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class RecipeDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Recipe]";

        private static Recipe GetRecipe(SqlDataReader reader)
        {
            return new Recipe(Convert.ToInt64(reader["Id"]),
                              Convert.ToInt64(reader["PostId"]),
                              Convert.ToInt64(reader["RecipeTypeId"]),
                              reader["Ingredients"].ToString(),
                              reader["Preparation"].ToString(),
                              Convert.ToInt32(reader["Portions"]),
                              Convert.ToInt32(reader["CookingTime"]),
                              Convert.ToDateTime(reader["CreateDateTime"]),
                              Convert.ToDateTime(reader["UpdateDateTime"]),
                              Convert.ToInt32(reader["Status"]));
        }

        public static RecipeFull GetRecipeFull(SqlDataReader reader)
        {
            return new RecipeFull(Convert.ToInt64(reader["Id"]),

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
                                  Convert.ToDateTime(reader["PublicationDateTime"]),
                                  Convert.ToInt32(reader["Status"]),
                                  null,   //ContactFull
                                  null,   //LinkFulls
                                  null,   //CommentFulls

                                  Convert.ToInt64(reader["RecipeTypeId"]),
                                  reader["Ingredients"].ToString(),
                                  reader["Preparation"].ToString(),
                                  Convert.ToInt32(reader["Portions"]),
                                  Convert.ToInt32(reader["CookingTime"]),
                                  Convert.ToInt32(reader["Status"]),

                                  null);  //Images);
        }


        // GET
        public async Task<List<Recipe>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                command.AddParam("@Status", SqlDbType.Int, status);

            List<Recipe> recipes = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Recipe recipe = GetRecipe(reader);
                         recipes.Add(recipe);
                    }
                }
            }
            return recipes;
        }

        public async Task<Recipe> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            Recipe recipe = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         recipe = GetRecipe(reader);
                    }
                }
            }
            return recipe;
        }

        // GET FULL
        public async Task<RecipeFull> GetFullById(long id, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(Lik.[Rank], -1) AS [Like]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.RecipeTypeId, {table}.Ingredients, {table}.Preparation, {table}.Portions, {table}.CookingTime, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId " +
                            " LEFT JOIN [D-Like] AS Lik ON Lik.PostId = Post.Id AND Lik.AppUserId = @LikeAppUserId " +
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
            command.AddParam("@Id", SqlDbType.BigInt, id);
            command.AddParam("@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

            RecipeFull recipeFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    recipeFull = GetRecipeFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        recipeFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    recipeFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    recipeFull.CommentFulls = commentFulls;
                }
            }

            return recipeFull;
        }

        public async Task<RecipeFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias," +
                             " Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(Lik.[Rank], -1) AS [Like]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.RecipeTypeId, {table}.Ingredients, {table}.Preparation, {table}.Portions, {table}.CookingTime, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId " +
                            " LEFT JOIN [D-Like] AS Lik ON Lik.PostId = Post.Id AND Lik.AppUserId = @LikeAppUserId " +
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
            command.AddParam("@PostId", SqlDbType.BigInt, postId);
            command.AddParam("@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

            RecipeFull recipeFull = null;
            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    recipeFull = GetRecipeFull(reader);

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        recipeFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    recipeFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    recipeFull.CommentFulls = commentFulls;
                }
            }

            return recipeFull;
        }

        public async Task<RecipeDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, 0 AS Favorite, -1 AS [Like], Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.RecipeTypeId, {table}.Ingredients, {table}.Preparation, {table}.Portions, {table}.CookingTime, {table}.Status" +
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
                command.AddParam("@Status", SqlDbType.Int, status);

            RecipeDataFull recipeDataFull = new RecipeDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<RecipeFull> recipeFulls = [];
                    while (await reader.ReadAsync())
                        recipeFulls.Add(GetRecipeFull(reader));
                    recipeDataFull.RecipeFulls = recipeFulls;

                    await reader.NextResultAsync();
                    List<ContactFull> contactFulls = [];
                    while (await reader.ReadAsync())
                        contactFulls.Add(ContactDB.GetContactFull(reader));
                    recipeDataFull.ContactFulls = contactFulls;

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    recipeDataFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    recipeDataFull.CommentFulls = commentFulls;
                }
            }

            return recipeDataFull;
        }

        // INSERT
        public async Task<long> Add(Recipe recipe)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, RecipeTypeId, Ingredients, Preparation, Portions, CookingTime, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @RecipeTypeId, @Ingredients, @Preparation, @Portions, @CookingTime, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('E'));
            command.AddParam("@PostId", SqlDbType.BigInt, recipe.PostId);
            command.AddParam("@RecipeTypeId", SqlDbType.BigInt, recipe.RecipeTypeId);
            command.AddParam("@Ingredients", SqlDbType.VarChar, recipe.Ingredients);
            command.AddParam("@Preparation", SqlDbType.VarChar, recipe.Preparation);
            command.AddParam("@Portions", SqlDbType.Int, recipe.Portions);
            command.AddParam("@CookingTime", SqlDbType.Int, recipe.CookingTime);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, recipe.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Recipe recipe)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, RecipeTypeId = @RecipeTypeId, Ingredients = @Ingredients, Preparation = @Preparation, Portions = @Portions, CookingTime = @CookingTime, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PostId", SqlDbType.BigInt, recipe.PostId);
            command.AddParam("@RecipeTypeId", SqlDbType.BigInt, recipe.RecipeTypeId);
            command.AddParam("@Ingredients", SqlDbType.VarChar, recipe.Ingredients);
            command.AddParam("@Preparation", SqlDbType.VarChar, recipe.Preparation);
            command.AddParam("@Portions", SqlDbType.Int, recipe.Portions);
            command.AddParam("@CookingTime", SqlDbType.Int, recipe.CookingTime);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, recipe.Status);
            command.AddParam("@Id", SqlDbType.BigInt, recipe.Id);

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
