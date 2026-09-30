using System;

namespace HeroServer
{
    public class Radio
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Radio() { }

        public Radio(long id, long postId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public Radio(RadioFull radioFull)
        {
            Id = radioFull.Id;
            PostId = radioFull.PostId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = radioFull.Status;
        }
    }
}
