using System;

namespace HeroServer
{
    public class Like
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public int Rank { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Like() { }

        public Like(long id, long postId, long appUserId, int rank, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            AppUserId = appUserId;
            Rank = rank;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
