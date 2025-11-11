
namespace HeroServer
{
    public class IdentityInfo
    {
        public Identity Identity { get; set; }
        public DpiPhoto DpiPhoto { get; set; }

        public IdentityInfo()
        {
        }

        public IdentityInfo(Identity identity, DpiPhoto dpiPhoto)
        {
            Identity = identity;
            DpiPhoto = dpiPhoto;
        }
    }
}
