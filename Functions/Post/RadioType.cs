using System;

namespace HeroServer
{
    public class RadioType
    {
        public long Id { get; set; }
        public long RadioId { get; set; }
        public long RadioTypeId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public RadioType() 
        {
        }

        public RadioType(long id, long radioId, long radioTypeId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            RadioId = radioId;
            RadioTypeId = radioTypeId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
