
using System;

namespace HeroServer
{
    public class CommentFeedRequest
    {
        // PARAMS
        public int Chunk { get; set; } = -1;
        public DateTime StartDateTime { get; set; }
        public int Direction { get; set; }
        public int Count { get; set; }

        // FILTERS
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public int Status { get; set; }
    }
}
