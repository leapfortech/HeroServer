using System;

namespace HeroServer
{
    public class Tale
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Tale() { }

        public Tale(long id, long postId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public Tale(TaleFull taleFull)
        {
            Id = taleFull.Id;
            PostId = taleFull.PostId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = taleFull.Status;
        }
    }
}
