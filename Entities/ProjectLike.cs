using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class ProjectLike
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int AppUserId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public ProjectLike()
        {
        }

        public ProjectLike(int id, int projectId, int appUserId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            ProjectId = projectId;
            AppUserId = appUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
