using System.Collections.Generic;

namespace HeroServer
{
    public class ProductFeedResponse(ProductFeedRequest request)
    {
        public int Chunk { get; set; } = request.Chunk;
        public int Direction { get; set; } = request.Direction;

        public List<ProductFeed> ProductFeeds { get; set; } = new List<ProductFeed>(request.Count);

        public int Total { get; set; } = 0;
    }
}
