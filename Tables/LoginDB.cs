using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class LoginDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);

        // SELECT
        public async Task<LoginAppInfo> GetLoginAppInfo(int appUserId, int webSysUserId, DateTime meetingStartDateTime, DateTime meetingEndDateTime)
        {
            //News
            String strCmd = // Referred Count
                            "SELECT COUNT(1) AS Count FROM [D-Referred] WHERE AppUserId = @AppUserId AND Status = 1;" +

                            // Identity
                            "SELECT * FROM [D-Identity] WHERE AppUserId = @AppUserId AND Status = 1; " +

                            // Address AppUser
                            "SELECT Adr.* FROM [D-Address] AS Adr INNER JOIN [J-AddressAppUser] AS AdrApp ON (AdrApp.AddressId = Adr.Id) WHERE AdrApp.AppUserId = @AppUserId AND AdrApp.Status = 1; " +

                            // Card
                            "SELECT * FROM [D-Card] WHERE AppUserId = @AppUserId AND Status = 1; " +

                            // Notification
                            "SELECT TOP (50) * FROM [D-Notification] WHERE WebSysUserId = @WebSysUserId AND NotificationStatusId = 1 ORDER BY DateTime DESC;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysUserId);

            LoginAppInfo loginAppInfo = new LoginAppInfo();

            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    ReferredCount referredCount = new ReferredCount();
                    if (await reader.ReadAsync())
                        referredCount.Count = Convert.ToInt32(reader["Count"]);

                    reader.NextResult();
                    if (await reader.ReadAsync())
                        referredCount.InvestmentCount = Convert.ToInt32(reader["InvestmentCount"]);

                    loginAppInfo.ReferredCount = referredCount;

                    reader.NextResult();
                    if (await reader.ReadAsync())
                        loginAppInfo.Identity = IdentityDB.GetIdentity(reader);

                    reader.NextResult();
                    if (await reader.ReadAsync())
                        loginAppInfo.Address = AddressDB.GetAddress(reader);

                    reader.NextResult();
                    if (await reader.ReadAsync())
                        loginAppInfo.Card = CardDB.GetCard(reader);

                    reader.NextResult();
                    List<Notification> notifications = [];
                    while (await reader.ReadAsync())
                        notifications.Add(NotificationDB.GetNotification(reader));
                    loginAppInfo.Notifications = notifications;
                }
            }

            return loginAppInfo;
        }
    }
}
