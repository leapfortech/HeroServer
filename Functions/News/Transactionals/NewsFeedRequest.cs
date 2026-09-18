using System;

namespace HeroServer
{
    public class NewsFeedRequest : PostFeedRequest
    {
        public long NewsTypeId { get; set; } = -1L;

        public NewsFeedRequest()
        {
            PostTypeId = PostType.News;
        }
    }
}
