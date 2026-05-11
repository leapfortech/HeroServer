
using System;

namespace HeroServer
{
    public class ReferredAllByCodeReq
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public String Code { get; set; }
        public int Status { get; set; }

        public ReferredAllByCodeReq()
        {
        }

        public ReferredAllByCodeReq(int page, int pageSize, string code, int status)
        {
            Page = page;
            PageSize = pageSize;
            Code = code;
            Status = status;
        }
    }
}