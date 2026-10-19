using System;

namespace HeroServer
{
    public class ProductFeedRequest : PostFeedRequest
    {
        public long[] ProductTypeIds { get; set; } = new long[] { -1L };

        public ProductFeedRequest()
        {
            PostTypeId = PostType.Product;
        }
    }
}
