using System;

namespace HeroServer
{
    public class MemoryFeedRequest : PostFeedRequest
    {
        public long MemoryTypeId { get; set; } = -1L;
        public MemoryFeedRequest()
        {
            PostTypeId = PostType.Memory;
        }
    }
}
