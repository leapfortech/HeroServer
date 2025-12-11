using System;

namespace HeroServer
{
    public class Link
    {
        public long Id { get; set; }
        public long LinkTypeId { get; set; }
        public long PostId { get; set; }
        public String Url { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Link() { }

        public Link(long id, long linkTypeId, long postId, String url, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            LinkTypeId = linkTypeId;
            PostId = postId;
            Url = url;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
