using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PostFeedResponse
    {
        public List<PostFull> PostFulls { get; set; } = new();

        public int Total { get; set; }

        public PostFeedResponse(int count)
        {
            PostFulls = new List<PostFull>(count);
        }
    }
}
