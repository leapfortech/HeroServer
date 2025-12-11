using System;

namespace HeroServer
{
    public class RadioListen
    {
        public long Id { get; set; }
        public long RadioId { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreateDateTime { get; set; }

        public RadioListen() { }

        public RadioListen(long id, long radioId, long appUserId, DateTime createDateTime)
        {
            Id = id;
            RadioId = radioId;
            AppUserId = appUserId;
            CreateDateTime = createDateTime;
        }
    }
}
