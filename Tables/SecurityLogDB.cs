using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class SecurityLogDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[Q-SecurityLog]";

        // SELECT


        // INSERT
        public async Task<long> Add(SecurityLog securityLog)
        {
            String strCmd = $"INSERT INTO {table} (DateTime, Type, Context, AuthEmail, AuthUserId," +
                            " AuthAppUserId, AppUserId, AppUserEmail)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@DateTime, @Type, @Context, @AuthEmail, @AuthUserId," +
                            " @AuthAppUserId, @AppUserId, @AppUserEmail)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@DateTime", SqlDbType.DateTime2, securityLog.DateTime);
            command.AddParam("@Type", SqlDbType.VarChar, securityLog.Type);
            command.AddParam("@Context", SqlDbType.VarChar, securityLog.Context);
            command.AddParam("@AuthEmail", SqlDbType.VarChar, securityLog.AuthEmail);
            command.AddParam("@AuthUserId", SqlDbType.VarChar, securityLog.AuthUserId);
            command.AddParam("@AuthAppUserId", SqlDbType.BigInt, securityLog.AuthAppUserId);
            command.AddParam("@AppUserId", SqlDbType.BigInt, securityLog.AppUserId);
            command.AddParam("@AppUserEmail", SqlDbType.VarChar, securityLog.AppUserEmail);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }
    }
}
