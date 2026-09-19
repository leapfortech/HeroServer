using System.Collections.Generic;

namespace HeroServer
{
    public class HappeningFeedResponse(HappeningFeedRequest request)
    {
        public int Chunk { get; set; } = request.Chunk;
        public int Direction { get; set; } = request.Direction;

        public List<HappeningFeed> HappeningFeeds { get; set; } = new List<HappeningFeed>(request.Count);

        public int Total { get; set; } = 0;
    }
}
