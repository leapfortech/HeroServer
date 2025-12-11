using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class TreatmentFull : PostFull
    {
        public long Id { get; set; }
        public String Ingredients { get; set; }
        public String Preparation { get; set; }
        public String Usage { get; set; }
        public String Annotation { get; set; }
        public int Status { get; set; }
        public List<DiseaseFull> DiseaseFulls { get; set; }


        public TreatmentFull(long id, long postId, long appUserId, String appUserAlias,
                             long postSubtypeId, long postCountryId, long postStateId,
                             String title, String summary, String description,
                             int imageCount, int likeCount, DateTime publicationDateTime,
                             int postStatusId,
                             String ingredients, String preparation, String usage, String annotation,
                             int status, List<DiseaseFull> diseaseFulls)
            : base(postId, appUserId, appUserAlias, postSubtypeId,
                   postCountryId, postStateId, title, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatusId)
        {
            Id = id;
            Ingredients = ingredients;
            Preparation = preparation;
            Usage = usage;
            Annotation = annotation;
            Status = status;
            DiseaseFulls = diseaseFulls ?? new List<DiseaseFull>();
        }
    }
}
