using System;

namespace HpbServer
{
    public class PuzzleCommentFull
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public String AppUserAlias { get; set; }
        public String Comment { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public PuzzleCommentFull()
        {
        }

        public PuzzleCommentFull(long id, long appUserId, String appUserAlias,
                                 String comment, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            AppUserAlias = appUserAlias;
            Comment = comment;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
