using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class NotificationDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Notification]";
        public static Notification GetNotification(SqlDataReader reader)
        {
            return new Notification(Convert.ToInt32(reader["Id"]), Convert.ToInt32(reader["WebSysUserId"]),
                                    reader["MessageId"].ToString(),
                                    reader["Title"].ToString(), reader["Body"].ToString(),
                                    reader["Action"].ToString(), reader["Information"].ToString(),
                                    reader["Parameter"].ToString(), Convert.ToInt32(reader["DisplayMode"]),
                                    Convert.ToDateTime(reader["DateTime"]),
                                    Convert.ToInt32(reader["NotificationStatusId"]));
        }

        // SELECT
        public async Task<Notification> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Notification notification = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        notification = GetNotification(reader);
                    }
                    
                }
            }
            return notification;
        }

        public async Task<List<Notification>> GetByWebSysUserId(int webSysUserId, int rowsCount)
        {
            String strCmd = "SELECT";

            if (rowsCount > 0)
                strCmd += " TOP (@RowsCount)";

            strCmd += $" * FROM {table}" +
                      " WHERE WebSysUserId = @WebSysUserId" +
                      " AND NotificationStatusId = 1" +
                      " ORDER BY DateTime DESC";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);

            if (rowsCount > 0)
                DBHelper.AddParam(command, "@RowsCount", SqlDbType.Int, rowsCount);

            List<Notification> notifications = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Notification notification = GetNotification(reader);
                        notifications.Add(notification);
                    }
                }
            }

            return notifications;
        }

        public async Task<List<Notification>> GetLost(int id, int webSysUserId)
        {
            String strCmd = $"SELECT * FROM {table}" +
                            " WHERE WebSysUserId = @WebSysUserId" +
                            " AND Id > @Id" +
                            " AND NotificationStatusId = 1" +
                            " ORDER BY DateTime ASC";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);
            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);

            List<Notification> notifications = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Notification notification = GetNotification(reader);
                        notifications.Add(notification);
                    }
                }
            }

            return notifications;
        }

        public async Task<String> GetLastInformation(int webSysUserId, String action)
        {
            String strCmd = $"SELECT TOP 1 Information FROM {table}" +
                            " WHERE WebSysUserId = @WebSysUserId" +
                            " AND Action = @Action" +
                            " AND NotificationStatusId = 1" +
                            " ORDER BY DateTime DESC";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);
            DBHelper.AddParam(command, "@Action", SqlDbType.VarChar, action);

            String information = "";
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        information = reader["Information"].ToString();
                    }
                }
            }

            return information;
        }

        
        // INSERT
        public async Task<int> Add(Notification notification)
        {
            String strCmd = $"INSERT INTO {table}(WebSysUserId, MessageId, Title, Body, Action, Information, Parameter, DisplayMode, DateTime, NotificationStatusId)" +
                               " OUTPUT INSERTED.Id" +
                               " VALUES (@WebSysUserId, @MessageId, @Title, @Body, @Action, @Information, @Parameter, @DisplayMode, @DateTime, @NotificationStatusId)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, notification.WebSysUserId);
            DBHelper.AddParam(command, "@MessageId", SqlDbType.VarChar, notification.MessageId);
            DBHelper.AddParam(command, "@Title", SqlDbType.VarChar, notification.Title);
            DBHelper.AddParam(command, "@Body", SqlDbType.VarChar, notification.Body);
            DBHelper.AddParam(command, "@Action", SqlDbType.VarChar, notification.Action);
            DBHelper.AddParam(command, "@Information", SqlDbType.VarChar, notification.Information);
            DBHelper.AddParam(command, "@Parameter", SqlDbType.VarChar, notification.Parameter);
            DBHelper.AddParam(command, "@DisplayMode", SqlDbType.Int, notification.DisplayMode);
            DBHelper.AddParam(command, "@DateTime", SqlDbType.DateTime2, notification.DateTime);
            DBHelper.AddParam(command, "@NotificationStatusId", SqlDbType.Int, notification.NotificationStatusId);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Notification notification)
        {
            String strCmd = $"UPDATE {table} SET WebSysUserId = @WebSysUserId, MessageId = @MessageId, Title = @Title, Body = @Body, Action = @Action, Information = @Information, Parameter = @Parameter, DisplayMode = @DisplayMode, DateTime = @DateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, notification.WebSysUserId);
            DBHelper.AddParam(command, "@MessageId", SqlDbType.VarChar, notification.MessageId);
            DBHelper.AddParam(command, "@Title", SqlDbType.VarChar, notification.Title);
            DBHelper.AddParam(command, "@Body", SqlDbType.VarChar, notification.Body);
            DBHelper.AddParam(command, "@Action", SqlDbType.VarChar, notification.Action);
            DBHelper.AddParam(command, "@Information", SqlDbType.VarChar, notification.Information);
            DBHelper.AddParam(command, "@Parameter", SqlDbType.VarChar, notification.Parameter);
            DBHelper.AddParam(command, "@DisplayMode", SqlDbType.Int, notification.DisplayMode);
            DBHelper.AddParam(command, "@DateTime", SqlDbType.DateTime2, notification.DateTime);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateMessageId(int id, String messageId, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET MessageId = @MessageId, NotificationStatusId = @NotificationStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@MessageId", SqlDbType.VarChar, messageId);
            DBHelper.AddParam(command, "@NotificationStatusId", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);


            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET NotificationStatusId = @NotificationStatusId" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@NotificationStatusId", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }


        // DELETE
        public async Task<int> DeleteAll()
        {
            String strCmd = $"DELETE FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteById(int id)
        {
            String strCmd = $"DELETE FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<int> DeleteByWebSysUserId(int webSysUserId)
        {
            String strCmd = $"DELETE FROM {table} WHERE WebSysUserId = @WebSysUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@WebSysUserId", SqlDbType.Int, webSysUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        //public async Task<int> DeleteByAppUserId(int appUserId)
        //{
        //    String strCmd = $"DELETE FROM {table}" +
        //                    $" INNER JOIN [DD-AppUser] ON ([DD-AppUser].WebSysUserId = {table}.WebSysUserId)" +
        //                     " WHERE [DD-AppUser].Id = @AppUserId";

        //    SqlCommand command = new SqlCommand(strCmd, conn);

        //    DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

        //    using (conn)
        //    {
        //        await conn.OpenAsync();
        //        return await command.ExecuteNonQueryAsync();
        //    }
        //}
    }
}
