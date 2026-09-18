using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HeroServer
{
    public static class DBHelper
    {
        // GENERIC
        public static void AddParam(this SqlCommand command, String name, SqlDbType sqlDbType, object value)
        {
            SqlParameter param = new SqlParameter()
            {
                ParameterName = name,
                SqlDbType = sqlDbType,
                Value = value ?? DBNull.Value
            };
            command.Parameters.Add(param);
        }

        // FEEDS
        private static readonly String[] feedTables = ["None", "Tale", "Recipe", "Treatment", "Radio", "Product", "Happening", "News", "Puzzle", "Memory"];

        private static int[] expirationTimes;

        public static void InitParams(int taleExpTime, int recipeExpTime, int treatmentExpTime, int radioExpTime, int productExpTime, int happeningExpTime, int newsExpTime, int memoryExpTime)
        {
            expirationTimes = [0, taleExpTime, recipeExpTime, treatmentExpTime, radioExpTime, productExpTime, happeningExpTime, newsExpTime, 0, memoryExpTime];
        }

        public static (String, String) GetFeedWheres(PostFeedRequest request, bool filterTypeId = false)
        {
            // FILTERS
            List<String> where = ["Post.PostTypeId = @PostTypeId"];

            if (request.AppUserId != -1L)
                where.Add("Post.AppUserId = @AppUserId");

            if (request.Status != -1)
                where.Add("Post.Status = @Status");

            if (request.CountryId != -1L)
                where.Add("Post.CountryId = @CountryId");

            if (request.StateId != -1L)
                where.Add("Post.StateId = @StateId");

            if (request.FavoriteAppUserId != 1L)
                where.Add($"EXISTS(SELECT 1 FROM [J-Favorite] AS Favorite WHERE Favorite.PostId = Post.Id AND Favorite.AppUserId = @FavoriteAppUserId)");

            if (request.SelectedAppUserId != -1L)
                where.Add($"EXISTS(SELECT 1 FROM [J-Selected] AS Selected WHERE Selected.PostId = Post.Id AND Selected.AppUserId = @SelectedAppUserId)");

            if (filterTypeId)
                where.Add($"{feedTables[request.PostTypeId]}.{feedTables[request.PostTypeId]}TypeId = @{feedTables[request.PostTypeId]}TypeId");

            // EXPIRATION
            if (expirationTimes[request.PostTypeId] > 0)
                where.Add($"Post.PublicationDateTime >= DATEADD(DAY, -{expirationTimes[request.PostTypeId]}, GETDATE()))");

            String whereCount = where.Count > 0 ? " WHERE " + String.Join(" AND ", where) : "";

            // PUBLICATION
            if (request.Direction == 2)
                where.Add("Post.PublicationDateTime < @StartDate");
            else
                where.Add("Post.PublicationDateTime > @StartDate");

            String whereFeed = where.Count > 0 ? " WHERE " + String.Join(" AND ", where) : "";

            return (whereFeed, whereCount);
        }

        public static String InitFeedCmd(int direction, String orderField)
        {
            if (direction == 1)
                return "WITH Posts AS" +
                       $" (SELECT ROW_NUMBER() OVER (ORDER BY Temp.{orderField} DESC) AS RowNumber, * FROM" +
                        " (SELECT TOP(@Count2)";

            return "SELECT TOP(@Count)";
        }

        public static String OrderFeedCmd(int direction, String orderField)
        {
            if (direction == 1)
                return $" ORDER BY Post.{orderField}) AS Temp)," +
                        " PostCount AS (SELECT COUNT(1) AS Total FROM Posts)" +
                        " SELECT * FROM Posts, PostCount" +
                        " WHERE RowNumber <= Total - @Count" +
                       $" ORDER BY {orderField}";
            return $" ORDER BY Post.{orderField} DESC;";
        }

        public static void AddFeedParams(this SqlCommand command, PostFeedRequest request, long typeId = -1)
        {
            command.AddParam("@PostTypeId", SqlDbType.BigInt, request.PostTypeId);
            command.AddParam("@StartDate", SqlDbType.DateTime2, request.StartDateTime);
            command.AddParam("@ReactionAppUserId", SqlDbType.BigInt, request.ReactionAppUserId);

            if (request.Direction == 1)
                command.AddParam("@Count2", SqlDbType.Int, request.Count * 2);
            command.AddParam("@Count", SqlDbType.Int, request.Count);

            if (request.AppUserId != -1L)
                command.AddParam("@AppUserId", SqlDbType.BigInt, request.AppUserId);

            if (request.CountryId != -1L)
                command.AddParam("@CountryId", SqlDbType.BigInt, request.CountryId);

            if (request.StateId != -1L)
                command.AddParam("@StateId", SqlDbType.BigInt, request.StateId);

            if (request.Status != -1)
                command.AddParam("@Status", SqlDbType.Int, request.Status);

            if (request.FavoriteAppUserId != -1L)
                command.AddParam("@FavoriteAppUserId", SqlDbType.BigInt, request.FavoriteAppUserId);

            if (request.SelectedAppUserId != -1L)
                command.AddParam("@SelectedAppUserId", SqlDbType.BigInt, request.SelectedAppUserId);

            if (typeId != -1)
                command.AddParam($"@{feedTables[request.PostTypeId]}TypeId", SqlDbType.BigInt, typeId);
        }
    }
}
