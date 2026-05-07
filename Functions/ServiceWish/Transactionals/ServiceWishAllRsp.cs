using System.Collections.Generic;

namespace HeroServer
{
    public class ServiceWishAllRsp
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<ServiceWishInfo> ServiceWishInfos { get; set; }

        public ServiceWishAllRsp()
        {
        }

        public ServiceWishAllRsp(int page, int totalPages, List<ServiceWishInfo> serviceWishInfos)
        {
            Page = page;
            TotalPages = totalPages;
            ServiceWishInfos = serviceWishInfos;
        }
    }
}