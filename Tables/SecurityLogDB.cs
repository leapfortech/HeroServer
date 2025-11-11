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
        public async Task<int> Add(SecurityLog securityLog)
        {
            String strCmd = $"INSERT INTO {table} (DateTime, Type, Context, AuthEmail, AuthUserId," +
                            " AuthAppUserId, AppUserId, AppUserEmail)" +
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@DateTime, @Type, @Context, @AuthEmail, @AuthUserId," +
                            " @AuthAppUserId, @AppUserId, @AppUserEmail)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@DateTime", SqlDbType.DateTime2, securityLog.DateTime);
            DBHelper.AddParam(command, "@Type", SqlDbType.VarChar, securityLog.Type);
            DBHelper.AddParam(command, "@Context", SqlDbType.VarChar, securityLog.Context);
            DBHelper.AddParam(command, "@AuthEmail", SqlDbType.VarChar, securityLog.AuthEmail);
            DBHelper.AddParam(command, "@AuthUserId", SqlDbType.VarChar, securityLog.AuthUserId);
            DBHelper.AddParam(command, "@AuthAppUserId", SqlDbType.Int, securityLog.AuthAppUserId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, securityLog.AppUserId);
            DBHelper.AddParam(command, "@AppUserEmail", SqlDbType.VarChar, securityLog.AppUserEmail);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }
    }
}
