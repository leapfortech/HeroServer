using System;

namespace HeroServer
{
    public class CommentPlaint
    {
        public long Id { get; set; }
        public long PlaintTypeId { get; set; }
        public long CommentId { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public CommentPlaint() { }

        public CommentPlaint(long id, long plaintTypeId, long commentId, long appUserId, DateTime createDateTime,
                             DateTime updateDateTime, int status)
        {
            Id = id;
            PlaintTypeId = plaintTypeId;
            CommentId = commentId;
            AppUserId = appUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
