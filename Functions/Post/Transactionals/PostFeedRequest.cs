using System;

namespace HeroServer
{
    public class PostFeedRequest
    {
        // PARAMS
        public int Chunk { get; set; } = -1;

        public DateTime StartDateTime { get; set; }
        public int Direction { get; set; } = -1;
        public int Count { get; set; } = 0;

        // LIKE
        public long LikeAppUserId { get; set; } = -1L;      // NOT in 1.5
        public long ReactionAppUserId { get; set; } = -1L;

        // FILTERS
        public long PostTypeId { get; set; } = -1L;
        public long AppUserId { get; set; } = -1L;
        public long CountryId { get; set; } = -1L;
        public long StateId { get; set; } = -1L;
        public int Status { get; set; } = -1;
    }
}
