using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class AddressProject
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int AddressId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }


        public AddressProject()
        {
        }

        public AddressProject(int id, int projectId, int addressId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            ProjectId = projectId;
            AddressId = addressId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
