using System;

namespace HeroServer
{
    public class Comment
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public String Message { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Comment() { }

        public Comment(long id, long postId, long appUserId, String message, DateTime createDateTime,
                             DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            AppUserId = appUserId;
            Message = message;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
