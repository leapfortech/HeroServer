using System.Collections.Generic;

namespace HeroServer
{
    public class TreatmentDataFull
    {
        public List<TreatmentFull> TreatmentFulls { get; set; }
        public List<PostFull> PostFulls { get; set; }
        public List<DiseaseFull> DiseaseFulls { get; set; }

        public TreatmentDataFull()
        {
        }

        public TreatmentDataFull(List<TreatmentFull> treatmentFulls,
                                 List<PostFull> postFulls,
                                 List<DiseaseFull> diseaseFulls)
        {
            TreatmentFulls = treatmentFulls;
            PostFulls = postFulls;
            DiseaseFulls = diseaseFulls;
        }
    }
}
