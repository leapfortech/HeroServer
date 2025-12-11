using System;

namespace HeroServer
{
    public class PostFull
    {
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public String AppUserAlias { get; set; }
        public long PostSubtypeId { get; set; }
        public long PostCountryId { get; set; }
        public long PostStateId { get; set; }
        public String Title { get; set; }
        public String Summary { get; set; }
        public String Description { get; set; }
        public int ImageCount { get; set; }
        public int LikeCount { get; set; }
        public DateTime PublicationDateTime { get; set; }
        public int PostStatus { get; set; }

        public PostFull()
        {
        }

        public PostFull(long postId, long appUserId, String appUserAlias,
                        long postSubtypeId, long postCountryId, long postStateId,
                        String title, String summary, String description,
                        int imageCount, int likeCount, DateTime publicationDateTime, int postStatus)
        {
            PostId = postId;
            AppUserId = appUserId;
            AppUserAlias = appUserAlias;
            PostSubtypeId = postSubtypeId;
            PostCountryId = postCountryId;
            PostStateId = postStateId;
            Title = title;
            Summary = summary;
            Description = description;
            ImageCount = imageCount;
            LikeCount = likeCount;
            PublicationDateTime = publicationDateTime;
            PostStatus = postStatus;
        }
    }
}
