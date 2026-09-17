using System;

namespace HeroServer
{
    public class Selected
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Selected()
        {
        }

        public Selected(long id, long postId, long appUserId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            AppUserId = appUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
