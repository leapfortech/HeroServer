using System;

namespace HeroServer
{
    public class Disease
    {
        public long Id { get; set; }
        public long TreatmentId { get; set; }
        public long DiseaseTypeId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Disease() { }

        public Disease(long id, long treatmentId, long diseaseTypeId, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            TreatmentId = treatmentId;
            DiseaseTypeId = diseaseTypeId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
