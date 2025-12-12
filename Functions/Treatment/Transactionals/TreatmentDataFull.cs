using System.Collections.Generic;

namespace HeroServer
{
    public class TreatmentDataFull
    {
        public List<TreatmentFull> TreatmentFulls { get; set; }
        public List<DiseaseFull> DiseaseFulls { get; set; }
        public List<ContactFull> ContactFulls { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }

        public TreatmentDataFull()
        {
        }

        public TreatmentDataFull(List<TreatmentFull> treatmentFulls,
                                 List<DiseaseFull> diseaseFulls,
                                 List<ContactFull> contactFulls,
                                 List<LinkFull> linkFulls,
                                 List<CommentFull> commentFulls)
        {
            TreatmentFulls = treatmentFulls;
            DiseaseFulls = diseaseFulls;
            ContactFulls = contactFulls;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
