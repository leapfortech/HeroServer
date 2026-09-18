using System;

namespace HeroServer
{
    public class RadioFeedRequest : PostFeedRequest
    {
        public RadioFeedRequest()
        {
            PostTypeId = PostType.Radio;
        }
    }
}
