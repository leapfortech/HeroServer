using System.Collections.Generic;

namespace HeroServer
{
    public class PostFeedResponse
    {
        public List<PostFull> PostFulls { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }

        public PostFeedResponse(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
            PostFulls = new List<PostFull>();
        }
    }
}
