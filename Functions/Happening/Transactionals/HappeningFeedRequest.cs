using System;

namespace HeroServer
{
    public class HappeningFeedRequest : PostFeedRequest
    {
        public long[] HappeningTypeIds { get; set; } = new long[] { -1L };

        public HappeningFeedRequest()
        {
            PostTypeId = PostType.Happening;
        }
    }
}
