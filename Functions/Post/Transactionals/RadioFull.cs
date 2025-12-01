using System;

namespace HpbServer
{
    public class RadioFull : PostFull
    {
        public long Id { get; set; }
        public String Link { get; set; }
        public int Status { get; set; }

        public RadioFull()
        {
        }

        public RadioFull(long id, long postId, long appUserId, String appUserAlias,
                         long postTypeId, long postSubtypeId, long originCountryId, long originStateId,
                         String title, String summary, String description,
                         int imageCount, int likesCount, DateTime publicationDateTime,
                         int postStatusId,
                         String link, int status)
            : base(postId, appUserId, appUserAlias, postTypeId, postSubtypeId,
                   originCountryId, originStateId, title, summary, description,
                   imageCount, likesCount, publicationDateTime, postStatusId)
        {
            Id = id;
            Link = link;
            Status = status;
        }
    }
}
