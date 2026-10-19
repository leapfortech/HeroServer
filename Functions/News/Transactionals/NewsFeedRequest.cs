using System;

namespace HeroServer
{
    public class NewsFeedRequest : PostFeedRequest
    {
        public long[] NewsTypeIds { get; set; } = new long[] { -1L };

        public NewsFeedRequest()
        {
            PostTypeId = PostType.News;
        }
    }
}
