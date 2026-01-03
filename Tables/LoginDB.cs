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
        public async Task<LoginAppInfo> GetLoginAppInfo(long appUserId, long webSysUserId,
                                                        long taleStatus = 1)
        {
            //News
            String strCmd = // Referred Count
                            "SELECT COUNT(1) AS Count FROM [D-Referred] WHERE AppUserId = @AppUserId AND Status = 1;" +

                            // Identity
                            "SELECT * FROM [D-Identity] AS Idt INNER JOIN [J-IdentityAppUser] AS IdtApp ON (IdtApp.IdentityId = Idt.Id) WHERE IdtApp.AppUserId = @AppUserId AND Status = 1; " +

                            // Address AppUser
                            "SELECT Adr.* FROM [D-Address] AS Adr INNER JOIN [J-AddressAppUser] AS AdrApp ON (AdrApp.AddressId = Adr.Id) WHERE AdrApp.AppUserId = @AppUserId AND AdrApp.Status = 1; " +

                            // Card
                            "SELECT * FROM [D-Card] WHERE AppUserId = @AppUserId AND Status = 1; " +

                            // Notification
                            "SELECT TOP (50) * FROM [D-Notification] WHERE WebSysUserId = @WebSysUserId AND NotificationStatusId = 1 ORDER BY DateTime DESC;";

            // Tale
            strCmd = "SELECT [D-Tale].Id, [D-Tale].PostId," +
                     " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostSubtypeId," +
                     " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                     " Post.ImageCount, Post.LikeCount, Post.PublicationDateTime, Post.PostStatus," +
                     " [D-Tale].Status" +
                     " FROM [D-Tale]" +
                     " INNER JOIN Post ON ([D-Tale].PostId = Post.PostId)" +
                     " INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)";


            if (taleStatus != -1)
                strCmd += $" WHERE [D-Tale].Status = @TaleStatus;";
            else
                strCmd += ";";

            strCmd += "SELECT Contact.Id, Contact.PostId, Contact.Name, Contact.Status" +
                      " FROM [D-Contact] AS Contact" +
                      " INNER JOIN [D-Tale] ON (Contact.PostId = [D-Tale].PostId)" +
                      " WHERE Contact.Status = 1";

            if (taleStatus != -1)
                strCmd += " AND [D-Tale].Status = @TaleStatus;";
            else
                strCmd += ";";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
                      " FROM [D-Link] AS Link" +
                      " INNER JOIN [D-Tale] ON (Link.PostId = [D-Tale].PostId)" +
                      " WHERE Link.Status = 1";

            if (taleStatus != -1)
                strCmd += " AND [D-Tale].Status = @TaleStatus;";
            else
                strCmd += ";";

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
                      " INNER JOIN [D-Tale]" +
                      " ON (Comment.PostId = [D-Tale].PostId)" +
                      " WHERE Comment.Status = 1";

            if (taleStatus != -1)
                strCmd += $" AND [D-Tale].Status = @TaleStatus;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.BigInt, appUserId);
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.BigInt, webSysUserId);

            if (taleStatus != -1)
                DBHelper.AddParam(command, "@TaleStatus", SqlDbType.Int, taleStatus);

            
            LoginAppInfo loginAppInfo = new LoginAppInfo();
            TaleDataFull taleDataFull = new TaleDataFull();

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

                    // Tale
                    reader.NextResult();
                    List<TaleFull> taleFulls = [];
                    while (await reader.ReadAsync())
                        taleFulls.Add(TaleDB.GetTaleFull(reader));
                    taleDataFull.TaleFulls = taleFulls;

                    await reader.NextResultAsync();
                    List<ContactFull> contactFulls = [];
                    while (await reader.ReadAsync())
                        contactFulls.Add(ContactDB.GetContactFull(reader));
                    taleDataFull.ContactFulls = contactFulls;

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    taleDataFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    taleDataFull.CommentFulls = commentFulls;
                }
            }

            loginAppInfo.TaleFulls = await TaleFunctions.GetFulls(taleDataFull);

            return loginAppInfo;
        }
    }
}
