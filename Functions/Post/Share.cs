using System;

namespace HeroServer
{
    public class Share
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreateDateTime { get; set; }

        public Share() { }

        public Share(long id, long postId, long appUserId, DateTime createDateTime)
        {
            Id = id;
            PostId = postId;
            AppUserId = appUserId;
            CreateDateTime = createDateTime;
        }
    }
}
