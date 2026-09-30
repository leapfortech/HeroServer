using System;

namespace HeroServer
{
    public class Contact
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String Name { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Contact() { }

        public Contact(long id, long postId, String name, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            Name = name;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public Contact(ContactFull contactFull)
        {
            Id = contactFull.Id;
            PostId = contactFull.PostId;
            Name = contactFull.Name;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = contactFull.Status;
        }
    }
}
