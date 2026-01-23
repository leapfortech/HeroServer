
namespace HeroServer
{
    public class PostFeedRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long PostSubtypeId { get; set; }
        public int Status { get; set; }

        public int Offset
        {
            get { return (Page - 1) * PageSize; }
        }
    }
}
