using System;

namespace HpbServer
{
    public class PostFull
    {
        public long PostId { get; set; }
        public long AppUserId { get; set; }
        public String AppUserAlias { get; set; }
        public long PostTypeId { get; set; }
        public long PostSubtypeId { get; set; }
        public long PostOriginCountryId { get; set; }
        public long PostOriginStateId { get; set; }
        public String Title { get; set; }
        public String Summary { get; set; }
        public String Description { get; set; }
        public int ImageCount { get; set; }
        public int LikesCount { get; set; }
        public DateTime PublicationDateTime { get; set; }
        public int PostStatus { get; set; }

        public PostFull()
        {
        }

        public PostFull(long postId, long appUserId, String appUserAlias,
                        long postTypeId, long postSubtypeId, long postOriginCountryId, long postOriginStateId,
                        String title, String summary, String description,
                        int imageCount, int likesCount, DateTime publicationDateTime, int postStatus)
        {
            PostId = postId;
            AppUserId = appUserId;
            AppUserAlias = appUserAlias;
            PostTypeId = postTypeId;
            PostSubtypeId = postSubtypeId;
            PostOriginCountryId = postOriginCountryId;
            PostOriginStateId = postOriginStateId;
            Title = title;
            Summary = summary;
            Description = description;
            ImageCount = imageCount;
            LikesCount = likesCount;
            PublicationDateTime = publicationDateTime;
            PostStatus = postStatus;
        }
    }
}
