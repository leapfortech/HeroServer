
using System;

namespace HeroServer
{
    public class UserInfoAllByAlias
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public String Alias { get; set; }
        public int Status { get; set; }

        public UserInfoAllByAlias()
        {
        }

        public UserInfoAllByAlias(int page, int pageSize, String alias, int status)
        {
            Page = page;
            PageSize = pageSize;
            Alias = alias;
            Status = status;
        }
    }
}