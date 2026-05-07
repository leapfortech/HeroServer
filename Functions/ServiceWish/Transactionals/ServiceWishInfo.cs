
namespace HeroServer
{
    public class ServiceWishInfo
    {
        public ServiceWish ServiceWish { get; set; }
        public ServiceWishUser ServiceWishUser { get; set; }

        public ServiceWishInfo()
        {
        }

        public ServiceWishInfo(ServiceWish serviceWish, ServiceWishUser serviceWishUser)
        {
            ServiceWish = serviceWish;
            ServiceWishUser = serviceWishUser;
        }
    }
}