using System;

namespace HeroServer
{
    public class LinkFull
    {
        public long Id { get; set; }
        public long LinkTypeId { get; set; }
        public long PostId { get; set; }
        public String Url { get; set; }
        public int Status { get; set; }

        public LinkFull()
        {
        }

        public LinkFull(long id, long linkTypeId, long postId, String url, int status)
        {
            Id = id;
            LinkTypeId = linkTypeId;
            PostId = postId;
            Url = url;
            Status = status;
        }
    }
}
