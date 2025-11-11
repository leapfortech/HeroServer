using System;

namespace HeroServer
{
    public class ProjectInformation
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProjectInformationTypeId { get; set; }
        public String Information { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public ProjectInformation()
        {
        }

        public ProjectInformation(int id, int projectId, int projectInformationTypeId, String information,
                                  DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            ProjectId = projectId;
            ProjectInformationTypeId = projectInformationTypeId;
            Information = information;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
