using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PostFeedResponse(int chunk, int count)
    {
        public int Chunk { get; set; } = chunk;

        public List<PostFull> PostFulls { get; set; } = new List<PostFull>(count);
        public int Total { get; set; } = 0;
    }
}
