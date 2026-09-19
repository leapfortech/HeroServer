using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class MemoryFeedResponse(MemoryFeedRequest request)
    {
        public int Chunk { get; set; } = request.Chunk;
        public int Direction { get; set; } = request.Direction;

        public List<MemoryFeed> MemoryFeeds { get; set; } = new List<MemoryFeed>(request.Count);

        public int Total { get; set; } = 0;
    }
}
