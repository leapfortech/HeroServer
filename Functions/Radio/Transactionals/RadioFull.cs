using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class RadioFull : PostFull
    {
        public long Id { get; set; }
        public int Favorite { get; set; } = 0;
        public int Status { get; set; }

        public List<RadioTypeFull> RadioTypeFulls { get; set; }
        public List<RadioLanguageFull> RadioLanguageFulls { get; set; }


        public RadioFull(long id, long postId, long appUserId, String appUserAlias,
                         long postTypeId, long postCountryId, long postStateId,
                         String title, String titleImage, String description,
                         int imageCount, int favoriteCount, int[] reactionCounts, long reactionPhraseId, int commentCount,
                         DateTime publicationDateTime, int postStatus,
                         AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls, List<String> images,
                         int favorite, int status, List<RadioTypeFull> radioTypeFulls, List<RadioLanguageFull> radioLanguageFulls)
            : base(postId, appUserId, appUserAlias, postTypeId, postCountryId, postStateId, title, titleImage, description,
                   imageCount, favoriteCount, reactionCounts, reactionPhraseId, commentCount, publicationDateTime, postStatus,
                   appUserInfo, contactFull, linkFulls, commentFulls, images)
        {
            Id = id;
            Favorite = favorite;
            Status = status;

            RadioTypeFulls = radioTypeFulls ?? new List<RadioTypeFull>();
            RadioLanguageFulls = radioLanguageFulls ?? new List<RadioLanguageFull>();
        }
    }
}
