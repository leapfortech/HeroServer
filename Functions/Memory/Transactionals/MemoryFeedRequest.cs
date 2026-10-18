using System;

namespace HeroServer
{
    public class MemoryFeedRequest : PostFeedRequest
    {
        public long[] MemoryTypeIds { get; set; } = new long[] { -1L };

        public MemoryFeedRequest()
        {
            PostTypeId = PostType.Memory;
        }
    }
}
