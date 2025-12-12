using System;

namespace HeroServer
{
    public class CommentFull
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public String AppUserAlias { get; set; }
        public String Message { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public CommentFull()
        {
        }

        public CommentFull(long id, long appUserId, String appUserAlias,
                                 String message, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            AppUserAlias = appUserAlias;
            Message = message;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
