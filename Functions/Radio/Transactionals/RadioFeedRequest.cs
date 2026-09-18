using System;

namespace HeroServer
{
    public class RadioFeedRequest
    {
        public int Chunk { get; set; } = -1;

        public DateTime StartDateTime { get; set; }
        public int Direction { get; set; } = -1;
        public int Count { get; set; } = 0;

        // FILTERS
        public long AppUserId { get; set; } = -1;
        public int Status { get; set; } = -1;
    }
}
