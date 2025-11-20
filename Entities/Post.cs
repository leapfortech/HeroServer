using System;

namespace HeroServer
{
    public class Post
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long PostTypeId { get; set; }
        public long PostSubtypeId { get; set; }
        public long OriginCountryId { get; set; }
        public long OriginStateId { get; set; }
        public String Title { get; set; }
        public String Summary { get; set; }
        public String Description { get; set; }
        public int ImageCount { get; set; }
        public int LikesCount { get; set; }
        public DateTime PublicationDateTime { get; set; }
        public DateTime? ApprovalDateTime { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Post() { }

        public Post(long id, long appUserId, long postTypeId, long postSubtypeId, long originCountryId,
                    long originStateId, String title, String summary, String description, int imageCount,
                    int likesCount, DateTime publicationDateTime, DateTime? approvalDateTime,
                    DateTime? expirationDateTime, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            PostTypeId = postTypeId;
            PostSubtypeId = postSubtypeId;
            OriginCountryId = originCountryId;
            OriginStateId = originStateId;
            Title = title;
            Summary = summary;
            Description = description;
            ImageCount = imageCount;
            LikesCount = likesCount;
            PublicationDateTime = publicationDateTime;
            ApprovalDateTime = approvalDateTime;
            ExpirationDateTime = expirationDateTime;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
