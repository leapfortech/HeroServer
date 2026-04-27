
namespace HeroServer
{
    public class ServiceWishAllByTypeReq
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long ServiceWishTypeId { get; set; }
        public int Status { get; set; }

        public ServiceWishAllByTypeReq()
        {
        }

        public ServiceWishAllByTypeReq(int page, int pageSize, long serviceWishTypeId, int status)
        {
            Page = page;
            PageSize = pageSize;
            ServiceWishTypeId = serviceWishTypeId;
            Status = status;
        }
    }
}