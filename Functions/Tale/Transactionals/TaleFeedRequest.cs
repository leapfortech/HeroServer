using System;

namespace HeroServer
{
    public class TaleFeedRequest : PostFeedRequest
    {
        public TaleFeedRequest()
        {
            PostTypeId = PostType.Tale;
        }
    }
}
