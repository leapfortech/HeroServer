using System;

namespace HeroServer
{
    public class ReferredHistoryRequest
    {
        public long AppUserId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public ReferredHistoryRequest()
        {
        }

        public ReferredHistoryRequest(long appUserId, DateTime dateStart, DateTime dateEnd)
        {
            AppUserId = appUserId;
            DateStart = dateStart;
            DateEnd = dateEnd;
        }
    }
}
