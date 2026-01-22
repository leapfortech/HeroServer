using System;

namespace HeroServer
{
    public class PostModerationRequest
    {
        public long PostId { get; set; }
        public long SubtypeId { get; set; }
    }
}