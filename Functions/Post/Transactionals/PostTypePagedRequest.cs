
using System;

namespace HeroServer
{
    public class PostTypePagedRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long PostTypeId { get; set; }
        public int Status { get; set; }

        public PostTypePagedRequest()
        {
        }

        public PostTypePagedRequest(int page, int pageSize, long postTypeId, int status)
        {
            Page = page;
            PageSize = pageSize;
            PostTypeId = postTypeId;
            Status = status;
        }
    }
}