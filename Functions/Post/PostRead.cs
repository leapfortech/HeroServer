using System;

namespace HeroServer
{
    public class PostRead
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreateDateTime { get; set; }

        public PostRead() 
        {
        }

        public PostRead(long id, long postId, long appUserId, DateTime createDateTime)
        {
            Id = id;
            PostId = postId;
            AppUserId = appUserId;
            CreateDateTime = createDateTime;
        }
    }
}
