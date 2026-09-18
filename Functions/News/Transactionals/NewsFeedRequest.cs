using System;

namespace HeroServer
{
    public class NewsFeedRequest : PostFeedRequest
    {
        public NewsFeedRequest()
        {
            PostTypeId = PostType.News;
        }
    }
}
