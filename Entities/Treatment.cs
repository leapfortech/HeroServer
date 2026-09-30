using System;

namespace HeroServer
{
    public class Treatment
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public String Ingredients { get; set; }
        public String Preparation { get; set; }
        public String Usage { get; set; }
        public String Annotation { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Treatment() { }

        public Treatment(long id, long postId, String ingredients, String preparation,
                         String usage, String annotation, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            PostId = postId;
            Ingredients = ingredients;
            Preparation = preparation;
            Usage = usage;
            Annotation = annotation;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public Treatment(TreatmentFull treatmentFull)
        {
            Id = treatmentFull.Id;
            PostId = treatmentFull.PostId;
            Ingredients = treatmentFull.Ingredients;
            Preparation = treatmentFull.Preparation;
            Usage = treatmentFull.Usage;
            Annotation = treatmentFull.Annotation;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            Status = treatmentFull.Status;
        }
    }
}
