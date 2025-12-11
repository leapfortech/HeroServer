using System;

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
                        String title, String summary, String description,
                        int imageCount, int likeCount, DateTime publicationDateTime,
                        int postStatusId, int status)
            : base(postId, appUserId, appUserAlias, postSubtypeId,
                   postCountryId, postStateId, title, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatusId)
        {
            Id = id;
            Status = status;
        }
    }
}
