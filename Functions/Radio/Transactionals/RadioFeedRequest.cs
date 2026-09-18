using System;

namespace HeroServer
{
    public class RadioFeedRequest : PostFeedRequest
    {
        public long FavoriteAppUserId { get; set; } = -1L;

        public RadioFeedRequest()
        {
            PostTypeId = PostType.Radio;
        }
    }
}
