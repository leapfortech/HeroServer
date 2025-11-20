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

        // GET
        public async Task<IEnumerable<Recipe>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Recipe> recipes = new List<Recipe>();
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

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

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

        // INSERT
        public async Task<long> Add(Recipe recipe)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, RecipeTypeId, Ingredients, Preparation, Portions, CookingTime, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @RecipeTypeId, @Ingredients, @Preparation, @Portions, @CookingTime, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, recipe.PostId);
            DBHelper.AddParam(command, "@RecipeTypeId", SqlDbType.BigInt, recipe.RecipeTypeId);
            DBHelper.AddParam(command, "@Ingredients", SqlDbType.VarChar, recipe.Ingredients);
            DBHelper.AddParam(command, "@Preparation", SqlDbType.VarChar, recipe.Preparation);
            DBHelper.AddParam(command, "@Portions", SqlDbType.Int, recipe.Portions);
            DBHelper.AddParam(command, "@CookingTime", SqlDbType.Int, recipe.CookingTime);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, recipe.Status);

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

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, recipe.PostId);
            DBHelper.AddParam(command, "@RecipeTypeId", SqlDbType.BigInt, recipe.RecipeTypeId);
            DBHelper.AddParam(command, "@Ingredients", SqlDbType.VarChar, recipe.Ingredients);
            DBHelper.AddParam(command, "@Preparation", SqlDbType.VarChar, recipe.Preparation);
            DBHelper.AddParam(command, "@Portions", SqlDbType.Int, recipe.Portions);
            DBHelper.AddParam(command, "@CookingTime", SqlDbType.Int, recipe.CookingTime);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, recipe.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, recipe.Id);

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
