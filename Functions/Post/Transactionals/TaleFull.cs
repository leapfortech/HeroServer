using System;

namespace HpbServer
{
    public class TaleFull : PostFull
    {
        public long Id { get; set; }
        public int Status { get; set; }

        public TaleFull()
        {
        }

        public TaleFull(long id, long postId, long appUserId, String appUserAlias,
                        long postTypeId, long postSubtypeId, long originCountryId, long originStateId,
                        String title, String summary, String description,
                        int imageCount, int likesCount, DateTime publicationDateTime,
                        int postStatusId, int status)
            : base(postId, appUserId, appUserAlias, postTypeId, postSubtypeId,
                   originCountryId, originStateId, title, summary, description,
                   imageCount, likesCount, publicationDateTime, postStatusId)
        {
            Id = id;
            Status = status;
        }
    }
}
