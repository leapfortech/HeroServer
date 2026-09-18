using System;
using System.Data;
using System.Data.SqlClient;

namespace HeroServer
{
    public static class DBHelper
    {
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

        public static void AddFeedParams(this SqlCommand command, PostFeedRequest request, (long Id, String Field)[] feedFields = null)
        {
            if (request.Direction == 1)
                command.AddParam("@Count2", SqlDbType.Int, request.Count * 2);
            command.AddParam("@Count", SqlDbType.Int, request.Count);

            command.AddParam("@PostTypeId", SqlDbType.BigInt, request.PostTypeId);

            command.AddParam("@LikeAppUserId", SqlDbType.BigInt, request.LikeAppUserId);
            command.AddParam("@ReactionAppUserId", SqlDbType.BigInt, request.ReactionAppUserId);

            if (request.AppUserId != -1L)
                command.AddParam("@AppUserId", SqlDbType.BigInt, request.AppUserId);

            if (request.CountryId != -1L)
                command.AddParam("@CountryId", SqlDbType.BigInt, request.CountryId);

            if (request.StateId != -1L)
                command.AddParam("@StateId", SqlDbType.BigInt, request.StateId);

            if (request.Status != -1)
                command.AddParam("@Status", SqlDbType.Int, request.Status);

            command.AddParam("@StartDate", SqlDbType.DateTime2, request.StartDateTime);

            if (feedFields != null)
                for (int i = 0; i < feedFields.Length; i++)
                    if (feedFields[i].Id > 1L)
                        command.AddParam($"@{feedFields[i].Field}", SqlDbType.BigInt, feedFields[i].Id);
        }
    }
}
