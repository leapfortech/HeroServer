using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class RadioFeedResponse(int chunk, int direction, int count)
    {
        public int Chunk { get; set; } = chunk;
        public int Direction { get; set; } = direction;

        public List<RadioFeed> RadioFeeds { get; set; } = new List<RadioFeed>(count);

        public int Total { get; set; } = 0;
    }
}
