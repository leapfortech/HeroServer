using System;

namespace HeroServer
{
    public class ProductFeedRequest : PostFeedRequest
    {
        public long ProductTypeId { get; set; } = -1L;

        public ProductFeedRequest()
        {
            PostTypeId = PostType.Product;
        }
    }
}
