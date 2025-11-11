using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PepAppUser
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int PepId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public PepAppUser()
        {
        }

        public PepAppUser(int id, int appUserId, int pepId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            PepId = pepId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
