using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class CpeAppUser
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int CpeId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public CpeAppUser()
        {
        }

        public CpeAppUser(int id, int appUserId, int cpeId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            CpeId = cpeId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
