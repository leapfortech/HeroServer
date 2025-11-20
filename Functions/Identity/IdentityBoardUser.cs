using System;

namespace HeroServer
{
    public class IdentityBoardUser
    {
        public long Id { get; set; }
        public long BoardUserId { get; set; }
        public long IdentityId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public IdentityBoardUser() { }

        public IdentityBoardUser(long id, long boardUserId, long identityId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            BoardUserId = boardUserId;
            IdentityId = identityId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
