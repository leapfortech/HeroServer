using System;

namespace HeroServer
{
    public class Referred
    {
        public long Id { get; set; }
        public String Code { get; set; }
        public long AppUserId { get; set; }
        public long IdentityId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public Referred()
        {
        }

        public Referred(long id, String code, long appUserId, long identityId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            Code = code;
            AppUserId = appUserId;
            IdentityId = identityId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
