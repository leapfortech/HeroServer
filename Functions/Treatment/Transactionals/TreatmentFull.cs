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
                             long postTypeId, long postCountryId, long postStateId,
                             String title, String titleImage, String description,
                             int imageCount, int favoriteCount, int[] reactionCounts, long reactionPhraseId, int commentCount,
                             DateTime publicationDateTime, int postStatus,
                             AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls, List<String> images,
                             String ingredients, String preparation, String usage, String annotation, int status, List<DiseaseFull> diseaseFulls)
            : base(postId, appUserId, appUserAlias, postTypeId, postCountryId, postStateId, title, titleImage, description,
                   imageCount, favoriteCount, reactionCounts, reactionPhraseId, commentCount, publicationDateTime, postStatus,
                   appUserInfo,contactFull, linkFulls, commentFulls, images)
        {
            Id = id;
            Ingredients = ingredients;
            Preparation = preparation;
            Usage = usage;
            Annotation = annotation;
            Status = status;
            DiseaseFulls = diseaseFulls ?? [];
        }
    }
}
