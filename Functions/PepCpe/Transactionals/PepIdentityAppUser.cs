using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PepIdentityAppUser
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int PepIdentityId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public PepIdentityAppUser()
        {
        }

        public PepIdentityAppUser(int id, int appUserId, int pepIdentityId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            PepIdentityId = pepIdentityId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public PepIdentityAppUser(int id, int appUserId, int pepIdentityId, int status)
        {
            Id = id;
            AppUserId = appUserId;
            PepIdentityId = pepIdentityId;
            Status = status;
        }
    }
}
