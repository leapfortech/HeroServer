using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PostFull
    {
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public String AppUserAlias { get; set; }
        public String Thumbnail { get; set; }
        public long PostTypeId { get; set; }
        public long PostCountryId { get; set; }
        public long PostStateId { get; set; }
        public String Title { get; set; }
        public String TitleImage { get; set; }
        public String Summary { get; set; }
        public String Description { get; set; }
        public int ImageCount { get; set; }
        public int[] ReactionCounts { get; set; }
        public long ReactionPhraseId { get; set; }
        public int CommentCount { get; set; }
        public DateTime PublicationDateTime { get; set; }
        public int PostStatus { get; set; }

        public AppUserInfo AppUserInfo { get; set; }
        public ContactFull ContactFull { get; set; }
        public List<LinkFull> LinkFulls { get; set; }
        public List<CommentFull> CommentFulls { get; set; }


        public PostFull()
        {
        }

        public PostFull(long postId, long appUserId, String appUserAlias, long postTypeId, long postCountryId, long postStateId, String title, String titleImage,
                        String summary, String description, int imageCount, int[] reactionCounts, long reactionPhraseId, int commentCount, DateTime publicationDateTime,
                        int postStatus, AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls)
        {
            PostId = postId;
            AppUserId = appUserId;
            AppUserAlias = appUserAlias;
            PostTypeId = postTypeId;
            PostCountryId = postCountryId;
            PostStateId = postStateId;
            Title = title;
            TitleImage = titleImage;
            Summary = summary;
            Description = description;
            ImageCount = imageCount;
            ReactionCounts = reactionCounts;
            ReactionPhraseId = reactionPhraseId;
            CommentCount = commentCount;
            PublicationDateTime = publicationDateTime;
            PostStatus = postStatus;

            AppUserInfo = appUserInfo;
            ContactFull = contactFull;
            LinkFulls = linkFulls;
            CommentFulls = commentFulls;
        }
    }
}
