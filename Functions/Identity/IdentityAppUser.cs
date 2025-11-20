using System;

namespace HeroServer
{
    public class IdentityAppUser
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long IdentityId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public IdentityAppUser() { }

        public IdentityAppUser(long id, long appUserId, long identityId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            IdentityId = identityId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
