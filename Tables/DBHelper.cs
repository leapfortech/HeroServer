using System;
using System.Data;
using System.Data.SqlClient;

namespace HeroServer
{
    public static class DBHelper
    {
        public static void AddParam(SqlCommand command, String name, SqlDbType sqlDbType, object value)
        {
            SqlParameter param = new SqlParameter()
            {
                ParameterName = name,
                SqlDbType = sqlDbType,
                Value = value ?? DBNull.Value
            };
            command.Parameters.Add(param);
        }
    }
}
