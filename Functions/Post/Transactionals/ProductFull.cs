using System;
using System.Collections.Generic;

namespace HpbServer
{
    public class ProductFull : PostFull
    {
        public long Id { get; set; }
        public long OriginCountryId { get; set; }
        public long SaleCountryId { get; set; }
        public long SaleStateId { get; set; }
        public long CurrencyId { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public int Status { get; set; }

        public ContactFull ContactFull { get; set; }
        public List<ProductReviewFull> ProductReviewFulls { get; set; }


        public ProductFull(long id, long postId, long appUserId, String appUserAlias,
                           long postTypeId, long postSubtypeId,
                           long postOriginCountryId, long postOriginStateId,
                           String title, String summary, String description,
                           int imageCount, int likesCount, DateTime publicationDateTime,
                           int postStatus,
                           long originCountryId, long saleCountryId, long saleStateId,
                           long currencyId, double price, double discountPrice,
                           int status,
                           ContactFull contactFull,
                           List<ProductReviewFull> productReviewFulls)
            : base(postId, appUserId, appUserAlias, postTypeId, postSubtypeId,
                   postOriginCountryId, postOriginStateId, title, summary, description,
                   imageCount, likesCount, publicationDateTime, postStatus)
        {
            Id = id;
            OriginCountryId = originCountryId;
            SaleCountryId = saleCountryId;
            SaleStateId = saleStateId;
            CurrencyId = currencyId;
            Price = price;
            DiscountPrice = discountPrice;
            Status = status;

            ContactFull = contactFull;
            ProductReviewFulls = productReviewFulls ?? new List<ProductReviewFull>();
        }
    }
}
