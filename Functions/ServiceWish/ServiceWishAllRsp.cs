using System.Collections.Generic;

namespace HeroServer
{
    public class ServiceWishAllRsp
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<ServiceWish> ServiceWishs { get; set; }

        public ServiceWishAllRsp()
        {
        }

        public ServiceWishAllRsp(int page, int totalPages, List<ServiceWish> serviceWishs)
        {
            Page = page;
            TotalPages = totalPages;
            ServiceWishs = serviceWishs;
        }
    }
}