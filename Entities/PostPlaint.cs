using System;

namespace HeroServer
{
    public class PostPlaint
    {
        public long Id { get; set; }
        public long PlaintTypeId { get; set; }
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public PostPlaint() { }

        public PostPlaint(long id, long plaintTypeId, long postId, long appUserId, DateTime createDateTime,
                          DateTime updateDateTime, int status)
        {
            Id = id;
            PlaintTypeId = plaintTypeId;
            PostId = postId;
            AppUserId = appUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
