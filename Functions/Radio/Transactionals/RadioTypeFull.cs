using System;

namespace HeroServer
{
    public class RadioTypeFull
    {
        public long Id { get; set; }
        public long RadioTypeId { get; set; }
        public int Status { get; set; }

        public RadioTypeFull()
        {
        }

        public RadioTypeFull(long id, long radioTypeId, int status)
        {
            Id = id;
            RadioTypeId = radioTypeId;
            Status = status;
        }
    }
}
