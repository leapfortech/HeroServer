
using System;

namespace HeroServer
{
    public class BoardUserAllByNameReq
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public String Name { get; set; }
        public int Status { get; set; }

        public BoardUserAllByNameReq()
        {
        }

        public BoardUserAllByNameReq(int page, int pageSize, String name, int status)
        {
            Page = page;
            PageSize = pageSize;
            Name = name;
            Status = status;
        }
    }
}