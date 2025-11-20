using System;

namespace HeroServer
{
    public class ProductReview
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long AppUserId { get; set; }
        public int Rating { get; set; }
        public String Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public ProductReview() { }

        public ProductReview(long id, long productId, long appUserId, int rating, String description,
                             DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            ProductId = productId;
            AppUserId = appUserId;
            Rating = rating;
            Description = description;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
