using System;

namespace HeroServer
{
    public class Radio
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String Link { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Radio() { }

        public Radio(long id, long postId, String link, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            Link = link;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
