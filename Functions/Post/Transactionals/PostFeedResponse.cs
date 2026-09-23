using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PostFeedResponse(int chunk, int direction, int count)
    {
        public int Chunk { get; set; } = chunk;
        public int Direction { get; set; } = direction;

        public List<PostFull> PostFulls { get; set; } = new List<PostFull>(count);

        // State
        public int Total { get; set; } = 0;
    }
}
