using System;

namespace HeroServer
{
    public class ContactFull
    {
        public long Id { get; set; }
        public String Name { get; set; }

        public ContactFull()
        {
        }

        public ContactFull(long id, String name)
        {
            Id = id;
            Name = name;
        }
    }
}
