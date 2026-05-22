using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class ProductFull : PostFull
    {
        public long Id { get; set; }
        public long ProductSubtypeId { get; set; }
        public long SaleCountryId { get; set; }
        public long SaleStateId { get; set; }
        public long CurrencyId { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public long DeliveryTypeId { get; set; }
        public String Annotation { get; set; }
        public int Status { get; set; }

        public List<ProductReviewFull> ProductReviewFulls { get; set; }

        public List<String> Images { get; set; }


        public ProductFull(long id, long postId, long appUserId, String appUserAlias,
                           long postTypeId,
                           long postCountryId, long postStateId,
                           String title, String titleImage, String summary, String description,
                           int imageCount, int favorite, int like, int likeCount, DateTime publicationDateTime,
                           int postStatus,
                           ContactFull contactFull,
                           List<LinkFull> linkFulls,
                           List<CommentFull> commentFulls,
                           long productSubtypeId, long saleCountryId, long saleStateId,
                           long currencyId, double price, double discountPrice,
                           long deliveryTypeId, String annotation,
                           int status,
                           List<ProductReviewFull> productReviewFulls,
                           List<String> images)
            : base(postId, appUserId, appUserAlias, postTypeId,
                   postCountryId, postStateId, title, titleImage, summary, description,
                   imageCount, favorite, like, likeCount, publicationDateTime, postStatus,
                   contactFull, linkFulls, commentFulls)
        {
            Id = id;
            ProductSubtypeId = productSubtypeId;
            SaleCountryId = saleCountryId;
            SaleStateId = saleStateId;
            CurrencyId = currencyId;
            Price = price;
            DiscountPrice = discountPrice;
            DeliveryTypeId = deliveryTypeId;
            Annotation = annotation;
            Status = status;

            ProductReviewFulls = productReviewFulls ?? new List<ProductReviewFull>();

            Images = images;
        }
    }
}
