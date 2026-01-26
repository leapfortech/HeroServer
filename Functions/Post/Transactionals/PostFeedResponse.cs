using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PostFeedResponse
    {
        public List<PostFull> PostFulls { get; set; } = new();

        public int Total { get; set; }

        // CURSORS
        public string NextCursor { get; set; }
        public string PrevCursor { get; set; }

        public PostFeedResponse(int pageSize)
        {
            PostFulls = new List<PostFull>(pageSize);
        }
    }
}
