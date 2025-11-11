
namespace HeroServer
{
    public class IdentityBoardInfo
    {
        public Identity Identity { get; set; }
        public DpiBoardPhoto DpiBoardPhoto { get; set; }

        public IdentityBoardInfo()
        {
        }

        public IdentityBoardInfo(Identity identity, DpiBoardPhoto dpiBoardPhoto)
        {
            Identity = identity;
            DpiBoardPhoto = dpiBoardPhoto;
        }
    }
}
