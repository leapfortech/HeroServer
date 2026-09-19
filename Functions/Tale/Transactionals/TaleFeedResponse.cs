using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class TaleFeedResponse(TaleFeedRequest request)
    {
        public int Chunk { get; set; } = request.Chunk;
        public int Direction { get; set; } = request.Direction;

        public List<TaleFeed> TaleFeeds { get; set; } = new List<TaleFeed>(request.Count);

        public int Total { get; set; } = 0;
    }
}
