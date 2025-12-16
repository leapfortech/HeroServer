using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class TaleFull : PostFull
    {
        public long Id { get; set; }
        public int Status { get; set; }

        public TaleFull()
        {
        }

        public TaleFull(long id, long postId, long appUserId, String appUserAlias,
                        long postSubtypeId, long postCountryId, long postStateId,
                        String title, String titleImage, String summary, String description,
                        int imageCount, int likeCount, DateTime publicationDateTime,
                        int postStatusId,
                        ContactFull contactFull,
                        List<LinkFull> linkFulls,
                        List<CommentFull> commentFulls,
                        int status)
            : base(postId, appUserId, appUserAlias, postSubtypeId,
                   postCountryId, postStateId, title, titleImage, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatusId,
                   contactFull, linkFulls, commentFulls)
        {
            Id = id;
            Status = status;
        }
    }
}
