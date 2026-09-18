using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class NewsFeedResponse(NewsFeedRequest request)
    {
        public int Chunk { get; set; } = request.Chunk;
        public int Direction { get; set; } = request.Direction;

        public List<NewsFeed> NewsFeeds { get; set; } = new List<NewsFeed>(request.Count);

        public int Total { get; set; } = 0;
    }
}
