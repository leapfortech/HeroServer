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

        public List<String> Images { get; set; }


        public TreatmentFull(long id, long postId, long appUserId, String appUserAlias,
                             long postTypeId, long postCountryId, long postStateId,
                             String title, String titleImage, String summary, String description,
                             int imageCount, int likeCount, DateTime publicationDateTime,
                             int postStatusId,
                             ContactFull contactFull,
                             List<LinkFull> linkFulls,
                             List<CommentFull> commentFulls,
                             String ingredients, String preparation, String usage, String annotation,
                             int status, List<DiseaseFull> diseaseFulls,
                             List<String> images)
            : base(postId, appUserId, appUserAlias, postTypeId,
                   postCountryId, postStateId, title, titleImage, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatusId,
                   contactFull, linkFulls, commentFulls)
        {
            Id = id;
            Ingredients = ingredients;
            Preparation = preparation;
            Usage = usage;
            Annotation = annotation;
            Status = status;
            DiseaseFulls = diseaseFulls ?? new List<DiseaseFull>();
            Images = images;
        }
    }
}
