using System;

namespace HeroServer
{
    public class Post
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long PostTypeId { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public int ImageCount { get; set; }
        public int FavoriteCount { get; set; }
        public DateTime PublicationDateTime { get; set; }
        public DateTime? ApprovalDateTime { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Post() { }

        public Post(long id, long appUserId, long postTypeId, long countryId,
                    long stateId, String title, String description, int imageCount,
                    int favoriteCount, DateTime publicationDateTime, DateTime? approvalDateTime,
                    DateTime? expirationDateTime, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            PostTypeId = postTypeId;
            CountryId = countryId;
            StateId = stateId;
            Title = title;
            Description = description;
            ImageCount = imageCount;
            FavoriteCount = favoriteCount;
            PublicationDateTime = publicationDateTime;
            ApprovalDateTime = approvalDateTime;
            ExpirationDateTime = expirationDateTime;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
