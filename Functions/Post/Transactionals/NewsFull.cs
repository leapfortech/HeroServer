using System;

namespace HpbServer
{
    public class NewsFull : PostFull
    {
        public long Id { get; set; }
        public long NewsTypeId { get; set; }
        public long OriginCountryId { get; set; }
        public long OriginStateId { get; set; }
        public String Source { get; set; }
        public String Url { get; set; }
        public DateTime? DateTime { get; set; }
        public int Status { get; set; }

        public NewsFull()
        {
        }

        public NewsFull(long id, long postId, long appUserId, String appUserAlias,
                        long postTypeId, long postSubtypeId,
                        long postOriginCountryId, long postOriginStateId,
                        String title, String summary, String description,
                        int imageCount, int likesCount, DateTime publicationDateTime,
                        int postStatus,
                        long newsTypeId, long originCountryId, long originStateId,
                        String source, String url, DateTime? dateTime,
                        int status)
            : base(postId, appUserId, appUserAlias, postTypeId, postSubtypeId,
                   postOriginCountryId, postOriginStateId, title, summary, description,
                   imageCount, likesCount, publicationDateTime, postStatus)
        {
            Id = id;
            NewsTypeId = newsTypeId;
            OriginCountryId = originCountryId;
            OriginStateId = originStateId;
            Source = source;
            Url = url;
            DateTime = dateTime;
            Status = status;
        }
    }
}
