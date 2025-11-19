using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public class NotificationFunctions
    {
        // GET
        public static async Task<Notification> GetById(long id)
        {
            return await new NotificationDB().GetById(id);
        }

        public static async Task<List<Notification>> GetByWebSysUserId(long webSysUserId, int rowsCount)
        {
            return await new NotificationDB().GetByWebSysUserId(webSysUserId, rowsCount);
        }

        public static async Task<List<Notification>> GetLost(long id, long webSysUserId)
        {
            return await new NotificationDB().GetLost(id, webSysUserId);
        }

        public static async Task<String> GetLastInformation(long webSysUserId, String action)
        {
            return await new NotificationDB().GetLastInformation(webSysUserId, action);
        }

        // ADD
        public static async Task<int> Add(Notification notification)
        {
            return await new NotificationDB().Add(notification);
        }

        // UPDATE
        public static async Task<bool> Update(Notification notification)
        {
            return await new NotificationDB().Update(notification);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new NotificationDB().UpdateStatus(id, status);
        }

        // DELETE
        public static async Task DeleteByWebSysUserId(long webSysUserId)
        {
            await new NotificationDB().DeleteByWebSysUserId(webSysUserId);
        }
    }
}
