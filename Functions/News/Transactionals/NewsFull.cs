using System;
using System.Collections.Generic;

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
                        String title, String titleImage, String summary, String description,
                        int imageCount, int likeCount, DateTime publicationDateTime,
                        int postStatus,
                        ContactFull contactFull,
                        List<LinkFull> linkFulls,
                        List<CommentFull> commentFulls,
                        long newsTypeId, String place,
                        String source, DateTime? dateTime,
                        int status)
            : base(postId, appUserId, appUserAlias, postSubtypeId,
                   postCountryId, postStateId, title, titleImage, summary, description,
                   imageCount, likeCount, publicationDateTime, postStatus,
                   contactFull, linkFulls, commentFulls)
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
