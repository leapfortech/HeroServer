using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PostFeedResponse(int chunk, int direction, int count)
    {
        public int Chunk { get; set; } = chunk;
        public int Direction { get; set; } = direction;

        public List<PostFull> PostFulls { get; set; } = new List<PostFull>(count);

        // Stats
        public int Total { get; set; } = 0;

        public long FirstPostId { get; set; } = -1;
        public DateTime FirstDateTime { get; set; }
        public long LastPostId { get; set; } = -1;
        public DateTime LastDateTime { get; set; }
    }
}
