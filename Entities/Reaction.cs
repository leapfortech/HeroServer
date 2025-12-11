using System;

namespace HeroServer
{
    public class Reaction
    {
        public long Id { get; set; }
        public long ReactionTypeId { get; set; }
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Reaction() { }

        public Reaction(long id, long reactionTypeId, long postId, long appUserId, DateTime createDateTime,
                        DateTime updateDateTime, int status)
        {
            Id = id;
            ReactionTypeId = reactionTypeId;
            PostId = postId;
            AppUserId = appUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
