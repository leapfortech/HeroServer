namespace HpbServer
{
    public class DiseaseFull
    {
        public long Id { get; set; }
        public long DiseaseTypeId { get; set; }
        public int Status { get; set; }

        public DiseaseFull()
        {
        }

        public DiseaseFull(long id, long diseaseTypeId, int status)
        {
            Id = id;
            DiseaseTypeId = diseaseTypeId;
            Status = status;
        }
    }
}
