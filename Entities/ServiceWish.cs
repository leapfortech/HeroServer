using System;

namespace HeroServer
{
    public class ServiceWish
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long ServiceTypeId { get; set; }
        public long ServiceOptionId { get; set; }
        public String Wish { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public ServiceWish()
        { 
        }

        public ServiceWish(long id, long appUserId, long serviceTypeId, long serviceOptionId, String wish, DateTime createDateTime,
                           DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            ServiceTypeId = serviceTypeId;
            ServiceOptionId = serviceOptionId;
            Wish = wish;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
