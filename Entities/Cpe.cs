using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class Cpe
    {
        public int Id { get; set; }
        public String ServiceType { get; set; }
        public String InstitutionName { get; set; }
        public String BeneficiaryName { get; set; }
        public String PositionType { get; set; }
        public DateTime CreateDatetime { get; set; }
        public DateTime UpdateDateTime { get; set; }


        public Cpe()
        {
        }

        public Cpe(int id, String serviceType, String institutionName, String beneficiaryName, String positionType, DateTime createDatetime, DateTime updateDateTime)
        {
            Id = id;
            ServiceType = serviceType;
            InstitutionName = institutionName;
            BeneficiaryName = beneficiaryName;
            PositionType = positionType;
            CreateDatetime = createDatetime;
            UpdateDateTime = updateDateTime;
        }
    }
}
