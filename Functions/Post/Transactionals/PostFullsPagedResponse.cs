
using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PostFullsPagedResponse
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<PostFull> PostFulls { get; set; }

        public PostFullsPagedResponse()
        {
        }

        public PostFullsPagedResponse(int page, int totalPages, List<PostFull> postFulls)
        {
            Page = page;
            TotalPages = totalPages;
            PostFulls = postFulls;
        }
    }
}