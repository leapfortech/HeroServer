using System;

namespace HpbServer
{
    public class ProductReviewFull
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public String AppUserAlias { get; set; }
        public int Rating { get; set; }
        public String Description { get; set; }
        public int Status { get; set; }

        public ProductReviewFull()
        {
        }

        public ProductReviewFull(long id, long appUserId, String appUserAlias,
                                 int rating, String description, int status)
        {
            Id = id;
            AppUserId = appUserId;
            AppUserAlias = appUserAlias;
            Rating = rating;
            Description = description;
            Status = status;
        }
    }
}
