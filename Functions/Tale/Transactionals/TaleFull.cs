using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class TaleFull : PostFull
    {
        public long Id { get; set; }
        public int Status { get; set; }

        public List<String> Images { get; set; }

        public TaleFull()
        {
        }

        public TaleFull(long id, long postId, long appUserId, String appUserAlias,
                        long postTypeId, long postCountryId, long postStateId,
                        String title, String titleImage, String summary, String description,
                        int imageCount, int favorite, int like, int likeCount, long reactionPhraseId,
                        DateTime publicationDateTime, int postStatus,
                        ContactFull contactFull,
                        List<LinkFull> linkFulls,
                        List<CommentFull> commentFulls,
                        int status,
                        List<String> images)
            : base(postId, appUserId, appUserAlias, postTypeId,
                   postCountryId, postStateId, title, titleImage, summary, description,
                   imageCount, favorite, like, likeCount, reactionPhraseId, publicationDateTime, postStatus,
                   contactFull, linkFulls, commentFulls)
        {
            Id = id;
            Status = status;
            Images = images;
        }
    }
}
