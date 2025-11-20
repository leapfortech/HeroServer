using System;

namespace HeroServer
{
    public class PuzzleComment
    {
        public long Id { get; set; }
        public long PuzzleId { get; set; }
        public long AppUserId { get; set; }
        public String Comment { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public PuzzleComment() { }

        public PuzzleComment(long id, long puzzleId, long appUserId, String comment, DateTime createDateTime,
                             DateTime updateDateTime, int status)
        {
            Id = id;
            PuzzleId = puzzleId;
            AppUserId = appUserId;
            Comment = comment;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
