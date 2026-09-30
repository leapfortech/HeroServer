namespace HeroServer
{
    public class DiseaseFull
    {
        public long Id { get; set; }
        public long TreatmentId { get; set; }
        public long DiseaseTypeId { get; set; }
        public int Status { get; set; }

        public DiseaseFull()
        {
        }

        public DiseaseFull(long id, long treatmentId, long diseaseTypeId, int status)
        {
            Id = id;
            TreatmentId = treatmentId;
            DiseaseTypeId = diseaseTypeId;
            Status = status;
        }
    }
}
