using System;
using System.Collections.Generic;

namespace HpbServer
{
    public class TreatmentFull : PostFull
    {
        public long Id { get; set; }
        public String Ingredients { get; set; }
        public String Preparation { get; set; }
        public String Usage { get; set; }
        public int Status { get; set; }
        public List<DiseaseFull> DiseaseFulls { get; set; }


        public TreatmentFull(long id, long postId, long appUserId, String appUserAlias,
                             long postTypeId, long postSubtypeId, long originCountryId, long originStateId,
                             String title, String summary, String description,
                             int imageCount, int likesCount, DateTime publicationDateTime,
                             int postStatusId,
                             String ingredients, String preparation, String usage,
                             int status, List<DiseaseFull> diseaseFulls)
            : base(postId, appUserId, appUserAlias, postTypeId, postSubtypeId,
                   originCountryId, originStateId, title, summary, description,
                   imageCount, likesCount, publicationDateTime, postStatusId)
        {
            Id = id;
            Ingredients = ingredients;
            Preparation = preparation;
            Usage = usage;
            Status = status;
            DiseaseFulls = diseaseFulls ?? new List<DiseaseFull>();
        }
    }
}
