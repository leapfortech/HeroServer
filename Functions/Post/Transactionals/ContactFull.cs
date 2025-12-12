using System;

namespace HeroServer
{
    public class ContactFull
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String Name { get; set; }
        public int Status { get; set; }

        public ContactFull()
        {
        }

        public ContactFull(long id, long postId, String name, int status)
        {
            Id = id;
            PostId = postId;
            Name = name;
            Status = status;
        }
    }
}
