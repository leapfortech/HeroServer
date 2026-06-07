using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class CommentFeedResponse(int chunk, int direction, int count)
    {
        public int Chunk { get; set; } = chunk;
        public int Direction { get; set; } = direction;

        public List<CommentFull> CommentFulls { get; set; } = new List<CommentFull>(count);

        // Stats
        public int Total { get; set; } = 0;

        public long FirstCommentId { get; set; } = -1;
        public DateTime FirstDateTime { get; set; }
        public long LastCommentId { get; set; } = -1;
        public DateTime LastDateTime { get; set; }
    }
}
