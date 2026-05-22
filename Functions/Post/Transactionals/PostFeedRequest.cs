
using System;

namespace HeroServer
{
    public class PostFeedRequest
    {
        // PARAMS
        public int Chunk { get; set; } = -1;

        public DateTime StartDateTime { get; set; }
        public int Direction { get; set; } = -1;
        public int Count { get; set; } = 20;

        // LIKE
        public long LikeAppUserId { get; set; } = -1;

        // FILTERS
        public long AppUserId { get; set; } = -1;
        public long PostTypeId { get; set; } = -1;
        public long CountryId { get; set; } = -1;
        public long StateId { get; set; } = -1;
        public int Status { get; set; } = -1;
    }
}
