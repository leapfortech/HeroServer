using System.Collections.Generic;

namespace HeroServer
{
    public class UserInfoAllRsp
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<UserInfo> UserInfos { get; set; }

        public UserInfoAllRsp()
        {
        }

        public UserInfoAllRsp(int page, int totalPages, List<UserInfo> userinfos)
        {
            Page = page;
            TotalPages = totalPages;
            UserInfos = userinfos;
        }
    }
}