using System;

namespace HeroServer
{
    public class NewsFull : PostFull
    {
        public long Id { get; set; }
        public long NewsTypeId { get; set; }
        public String Place { get; set; }
        public String Source { get; set; }
        public DateTime? DateTime { get; set; }
        public int Status { get; set; }

        public NewsFull()
        {
        }

        public NewsFull(long id, long postId, long appUserId, String appUserAlias,
                        long postSubtypeId,
                        long postCountryId, long postStateId,
                        String title, String summary, String description,
                        int imageCount, int likeCount, DateTime publicationDateTime,
                        int postStatus, long newsTypeId, String place,
                        String source, DateTime? dateTime,
                        int status)
            : base(postId, appUserId, appUserAlias, postSubtypeId,
                   postCountryId, postStateId, title, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatus)
        {
            Id = id;
            NewsTypeId = newsTypeId;
            Place = place;
            Source = source;
            DateTime = dateTime;
            Status = status;
        }
    }
}
