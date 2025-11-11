using System;

namespace HeroServer
{
    public class ReferredHistoryRequest
    {
        public int AppUserId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public ReferredHistoryRequest()
        {
        }

        public ReferredHistoryRequest(int appUserId, DateTime dateStart, DateTime dateEnd)
        {
            AppUserId = appUserId;
            DateStart = dateStart;
            DateEnd = dateEnd;
        }
    }
}
