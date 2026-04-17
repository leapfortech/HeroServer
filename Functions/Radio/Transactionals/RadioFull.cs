using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class RadioFull : PostFull
    {
        public long Id { get; set; }
        public long CountryId { get; }
        public int Status { get; set; }

        public List<RadioTypeFull> RadioTypeFulls { get; set; }
        public List<RadioLanguageFull> RadioLanguageFulls { get; set; }

        public List<String> Images { get; set; }

        public RadioFull()
        {
        }

        public RadioFull(long id, long postId, long appUserId, String appUserAlias,
                         long postTypeId, long postCountryId, long postStateId,
                         String title, String titleImage, String summary, String description,
                         int imageCount, int likeCount, DateTime publicationDateTime,
                         int postStatusId,
                         ContactFull contactFull,
                         List<LinkFull> linkFulls,
                         List<CommentFull> commentFulls,
                         int status,
                         List<RadioTypeFull> radioTypeFulls,
                         List<RadioLanguageFull> radioLanguageFulls,
                         List<String> images)
            : base(postId, appUserId, appUserAlias, postTypeId,
                   postCountryId, postStateId, title, titleImage, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatusId,
                   contactFull, linkFulls, commentFulls)
        {
            Id = id;
            CountryId = postCountryId;
            Status = status;

            RadioTypeFulls = radioTypeFulls ?? new List<RadioTypeFull>();
            RadioLanguageFulls = radioLanguageFulls ?? new List<RadioLanguageFull>();
            Images = images;
        }
    }
}
